using Microsoft.Extensions.DependencyInjection;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace Manulife.DNC.MSAD.WS.EventService.Models
{
    public static class AppQuartzExtensioin
    {
        public static void UseQuartz(this IServiceCollection services, params Type[] jobs)
        {
            services.AddSingleton<IJobFactory, QuartzJobFactory>();

            foreach (var serviceDescriptor in jobs.Select(jobType => new ServiceDescriptor(jobType, jobType, ServiceLifetime.Singleton)))
            {
                services.Add(serviceDescriptor);
            }

            services.AddSingleton(provider =>
            {
                var props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };

                var schedulerFactory = new StdSchedulerFactory(props);
                var scheduler = schedulerFactory.GetScheduler().Result;
                scheduler.JobFactory = provider.GetService<IJobFactory>();
                scheduler.Start();

                return scheduler;
            });
        }
    }
}
