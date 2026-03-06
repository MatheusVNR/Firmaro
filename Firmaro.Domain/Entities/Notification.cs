namespace Firmaro.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public DateTime? SentAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}