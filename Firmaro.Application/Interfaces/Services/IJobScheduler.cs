namespace Firmaro.Application.Interfaces.Services
{
    public interface IJobScheduler
    {
        void ScheduleReminder(Guid appointmentId, DateTimeOffset scheduledTime);
    }
}
