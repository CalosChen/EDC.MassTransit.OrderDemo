using Quartz;
using System;

namespace Manulife.DNC.MSAD.WS.EventService.Models
{
    public static class QuartzServiceUtil
    {
        public async static void StartJob<TJob>(IScheduler scheduler, TimeSpan interval)
            where TJob: IJob
        {
            var jobName = typeof(TJob).FullName;

            var job = JobBuilder.Create<TJob>()
                .WithIdentity(jobName)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{jobName}.trigger")
                .StartNow()
                .WithSimpleSchedule(scheduleBuilder =>
                    scheduleBuilder
                        .WithInterval(interval)
                        .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
