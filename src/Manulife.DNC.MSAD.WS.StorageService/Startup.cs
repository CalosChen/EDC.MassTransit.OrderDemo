using GreenPipes;
using Manulife.DNC.MSAD.WS.EventService.Models;
using Manulife.DNC.MSAD.WS.StorageService.Models;
using Manulife.DNC.MSAD.WS.StorageService.Repositories;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace Manulife.DNC.MSAD.WS.StorageService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Swagger
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc(Configuration["Service:DocName"], new Info
                {
                    Title = Configuration["Service:Title"],
                    Version = Configuration["Service:Version"],
                    Description = Configuration["Service:Description"],
                    Contact = new Contact
                    {
                        Name = Configuration["Service:Contact:Name"],
                        Email = Configuration["Service:Contact:Email"]
                    }
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, Configuration["Service:XmlFile"]);
                s.IncludeXmlComments(xmlPath);
            });

            // EFCore
            services.AddDbContextPool<OrderDbContext>(
                options => options.UseSqlServer(Configuration["DB:OrderDB"]));

            // Repository
            services.AddScoped<IStorageRepository, StorageRepsitory>();
            services.AddScoped<StoreageOrderEventHandler>();

            // MassTransit
            services.AddMassTransit(c =>
            {
                c.AddConsumer<StoreageOrderEventHandler>();
            });
        }

        public static IBusControl BusControl { get; private set; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            // swagger
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "doc/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint($"/doc/{Configuration["Service:DocName"]}/swagger.json",
                    $"{Configuration["Service:Name"]} {Configuration["Service:Version"]}");
            });

            // Register Bus
            BusControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(Configuration["MQ:Host"]), hst =>
                {
                    hst.Username(Configuration["MQ:UserName"]);
                    hst.Password(Configuration["MQ:Password"]);
                });

                cfg.ReceiveEndpoint(host, Configuration["MQ:Queues:Storage"], e =>
                {
                    // Retry
                    e.UseRetry(ret =>
                    {
                        ret.Interval(3, TimeSpan.FromSeconds(10)); // 每隔10s重试一次，最多重试3次
                    });

                    // RateLimit
                    e.UseRateLimit(1000, TimeSpan.FromSeconds(100)); // 100s内限1000次访问请求

                    // CircuitBreaker
                    e.UseCircuitBreaker(cb =>
                    {
                        cb.TrackingPeriod = TimeSpan.FromMinutes(1); // 跟踪周期：1min
                        cb.TripThreshold = 15; // 失败比例达到15%后才会打开熔断器
                        cb.ActiveThreshold = 5; // 至少发生5次请求后才会打开熔断器
                        cb.ResetInterval = TimeSpan.FromMinutes(5); // 熔断时间间隔：5mins
                    });

                    e.LoadFrom(serviceProvider);
                });
            });

            // Register Start & Stop for Bus
            lifetime.ApplicationStarted.Register(BusControl.Start);
            lifetime.ApplicationStarted.Register(BusControl.Stop);
        }
    }
}
