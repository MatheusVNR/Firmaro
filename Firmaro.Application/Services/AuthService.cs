using Firmaro.Application.DTOs;
using Firmaro.Application.Interfaces.Repositories;
using Firmaro.Application.Interfaces.Services;
using Firmaro.Domain.Entities;

namespace Firmaro.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IAutomationSettingsService _automationSettingsService;

        public AuthService(IUserRepository userRepository, 
                           IPasswordHasher passwordHasher, 
                           ITokenGenerator tokenGenerator,
                           IAutomationSettingsService automationSettingsService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _automationSettingsService = automationSettingsService;
        }


        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            User? existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null) throw new Exception("E-mail já cadastrado.");

            User user = new()
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                BusinessName = request.BusinessName,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _automationSettingsService.CreateDefaultSettingsAsync(user.Id);

            string token = _tokenGenerator.GenerateToken(user.Id, user.Email);
            return new AuthResponse(user.Id, user.Name, token);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            User? user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new Exception("Credenciais inválidas.");

            string token = _tokenGenerator.GenerateToken(user.Id, user.Email);
            return new AuthResponse(user.Id, user.Name, token);
        }
    }
}
