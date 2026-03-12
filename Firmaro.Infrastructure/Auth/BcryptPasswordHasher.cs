using Firmaro.Application.Interfaces.Services;

namespace Firmaro.Infrastructure.Auth
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password) 
            => BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(string password, string hash) 
            => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
