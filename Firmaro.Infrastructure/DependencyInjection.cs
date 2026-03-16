using Firmaro.Application.Interfaces.Repositories;
using Firmaro.Application.Interfaces.Services;
using Firmaro.Infrastructure.Auth;
using Firmaro.Infrastructure.Data;
using Firmaro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Firmaro.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext(configuration);
            services.AddRepositories();
            services.AddAuthProviders();

            return services;
        }

        private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FirmaroDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                       .UseSnakeCaseNamingConvention());
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        }

        private static void AddAuthProviders(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
        }
    }
}
