using GreenPipes;
using Manulife.DNC.MSAD.WS.Events;
using Manulife.DNC.MSAD.WS.EventService.Models;
using Manulife.DNC.MSAD.WS.EventService.Repositories;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;

namespace Manulife.DNC.MSAD.WS.EventService
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
            //services.AddMvc();

            // EFCore
            services.AddDbContextPool<OrderDbContext>(
                options => options.UseSqlServer(Configuration["DB:OrderDB"]));

            // Dapper-ConnString
            services.AddSingleton(Configuration["DB:OrderDB"]);

            // Repository
            services.AddSingleton<IEventRepository<IOrderEventEntity>, OrderEventDapperRepository>();

            // Quartz
            services.UseQuartz(typeof(OrderEventJob));

            // MassTransit
            services.AddMassTransit(c =>
            {
                c.AddConsumer<OrderEventHandler>();
            });
        }

        public static IBusControl BusControl { get; private set; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env, IApplicationLifetime lifetime, IScheduler scheduler)
        {
            //app.UseMvc();

            // Register EventBus
            BusControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(Configuration["MQ:Host"]), hst =>
                {
                    hst.Username(Configuration["MQ:UserName"]);
                    hst.Password(Configuration["MQ:Password"]);
                });

                // Retry
                cfg.UseRetry(ret =>
                {
                    ret.Interval(3, TimeSpan.FromSeconds(10)); // 每隔10s重试一次，最多重试3次
                });

                // RateLimit
                cfg.UseRateLimit(1000, TimeSpan.FromSeconds(100)); // 100s内限1000次访问请求

                // CircuitBreaker
                cfg.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1); // 跟踪周期：1min
                    cb.TripThreshold = 15; // 失败比例达到15%后才会打开熔断器
                    cb.ActiveThreshold = 5; // 至少发生5次请求后才会打开熔断器
                    cb.ResetInterval = TimeSpan.FromMinutes(5); // 熔断时间间隔：5mins
                });

                cfg.ReceiveEndpoint(host, Configuration["MQ:Queues:Order"], e => 
                {
                    e.LoadFrom(serviceProvider);
                });
            });

            // Register Start & Stop for Bus
            lifetime.ApplicationStarted.Register(BusControl.Start);
            lifetime.ApplicationStarted.Register(BusControl.Stop);

            // Scheduler Job
            QuartzServiceUtil.StartJob<OrderEventJob>(scheduler, TimeSpan.FromSeconds(30));
        }
    }
}
