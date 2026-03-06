
namespace Firmaro.Domain.Entities
{
    public class AutomationSettings
    {
        public Guid UserId { get; set; }
        public bool SendDayBefore { get; set; }
        public int SendHoursBefore { get; set; }
        public bool RequireConfirmation { get; set; }
    }
}