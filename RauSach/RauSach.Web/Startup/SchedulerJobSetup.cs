using Quartz;
using RauSach.Scheduler;
using Quartz.Impl;

namespace RauSach.Web.Startup
{
    public static class SchedulerJobSetup
    {
        public static async Task RegisterJobAsync(this IServiceCollection services)
        {
            services.AddTransient<DeliveryDetection>();

            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = await schedulerFactory.GetScheduler();
            scheduler.JobFactory = new QuartzJobFactory(services);

            await scheduler.ScheduleJob(
                JobBuilder.Create<DeliveryDetection>().Build(),
                TriggerBuilder.Create().WithSimpleSchedule(s => s.WithIntervalInMinutes(120).RepeatForever()).WithIdentity(nameof(DeliveryDetection)).Build());

            await scheduler.Start();
        }
    }
}
