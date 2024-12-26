using Quartz;
using Quartz.Spi;

namespace RauSach.Web.Startup
{
    public class QuartzJobFactory : IJobFactory
    {
        private readonly IServiceCollection _serviceProvider;
        public QuartzJobFactory(IServiceCollection serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                return (IJob)_serviceProvider.BuildServiceProvider().GetService(bundle.JobDetail.JobType);
            }
            catch (Exception e)
            {
                throw new SchedulerException($"Problem while instantiating job instance type {bundle.JobDetail.JobType}", e);
            }
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
