
namespace Firmaro.Application.DTOs
{
    public record RegisterRequest(string Name, string Email, string Phone, string BusinessName, string Password);
    public record LoginRequest(string Email, string Password);
    public record AuthResponse(Guid UserId, string Name, string Token);
}
