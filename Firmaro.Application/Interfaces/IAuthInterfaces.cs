using Firmaro.Application.DTOs;

namespace Firmaro.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public interface ITokenGenerator
    {
        string GenerateToken(Guid userId, string email);
    }

    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
