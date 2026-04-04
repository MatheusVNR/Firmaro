using Firmaro.Application.Interfaces.Repositories;
using Firmaro.Application.Interfaces.Services;
using Firmaro.Infrastructure.Auth;
using Firmaro.Infrastructure.Data;
using Firmaro.Infrastructure.Jobs;
using Firmaro.Infrastructure.Repositories;
using Firmaro.Infrastructure.Services;
using Hangfire;
using Hangfire.PostgreSql;
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
            services.AddHangfire(configuration);
            services.AddRepositories();
            services.AddAuthProviders();
            services.AddServices();

            return services;
        }

        private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FirmaroDbContext>(options =>
                options.UseNpgsql(GetConnectionString(configuration))
                       .UseSnakeCaseNamingConvention());
        }

        private static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(GetConnectionString(configuration))));

            services.AddHangfireServer();
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

        private static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IJobScheduler, HangfireJobScheduler>();
        }

        #region Aux Methods
        private static string? GetConnectionString(IConfiguration configuration)
        {
            return configuration.GetConnectionString("DefaultConnection");
        }
        #endregion
    }
}
