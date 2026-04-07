namespace Firmaro.Application.DTOs
{
    public record UpdateAutomationSettingsRequest(bool SendDayBefore, int SendHoursBefore, bool RequireConfirmation);
}
