namespace Firmaro.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendReminderAsync(Guid appointmentId);
    }
}
