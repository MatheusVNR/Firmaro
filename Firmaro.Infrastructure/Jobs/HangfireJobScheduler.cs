using Firmaro.Application.Interfaces.Services;
using Hangfire;

namespace Firmaro.Infrastructure.Jobs
{
    public class HangfireJobScheduler : IJobScheduler
    {
        private readonly IBackgroundJobClient _backgroundJobs;

        public HangfireJobScheduler(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobs = backgroundJobClient;
        }


        public void ScheduleReminder(Guid appointmentId, DateTimeOffset scheduledTime)
        {
            //DateTimeOffset enqueueAt = new(scheduledTime);

            _backgroundJobs.Schedule<INotificationService>(service => service.SendReminderAsync(appointmentId), scheduledTime);
        }
    }
}
