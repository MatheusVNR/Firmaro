namespace Firmaro.Application.DTOs
{
    public record CreateAppointmentRequest(string ClientName, string ClientPhone, DateTime DateTime);
    public record UpdateAppointmentRequest(string ClientName, string ClientPhone, DateTime DateTime);
    //public record AppointmentResponse(Guid Id, string ClientName, string ClientPhone, DateTime DateTime, string Status);
    public record AppointmentResponse(Guid Id, string ClientName, string ClientPhone, DateTimeOffset DateTime, string Status);
}
