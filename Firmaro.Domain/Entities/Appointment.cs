using Firmaro.Domain.Entities.Base;
using Firmaro.Domain.Enums;

namespace Firmaro.Domain.Entities
{
    public class Appointment : Entity
    {
        public Guid UserId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        //public DateTime DateTime { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string ConfirmationToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}