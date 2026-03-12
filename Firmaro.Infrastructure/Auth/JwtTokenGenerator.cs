using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Firmaro.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Firmaro.Infrastructure.Auth
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _config;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _config = configuration;
        }

        
        public string GenerateToken(Guid userId, string email)
        {
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            Claim[] claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];

            JwtSecurityToken token = new(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
