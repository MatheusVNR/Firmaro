using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Firmaro.Api.Extensions
{
    public static class ApiConfigurationExtensions
    {
        public static IServiceCollection AddNativeOpenApi(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

            return services;
        }

        public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
                    };
                });

            return services;
        }

        public static WebApplication UseHangfireDashboardConfiguration(this WebApplication app, IConfiguration configuration)
        {
            string? hangfireUser = configuration["Hangfire:DashboardUser"];
            string? hangfirePass = configuration["Hangfire:DashboardPassword"];

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[]
                {
                    new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        RequireSsl = false, // Mudar para true em produção
                        SslRedirect = false,
                        LoginCaseSensitive = true,
                        Users = new []
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login = hangfireUser,
                                PasswordClear = hangfirePass
                            }
                        }
                    })
                }
            });

            RecurringJob.AddOrUpdate("job-de-teste-inicial",
                () => Console.WriteLine($"[Hangfire] Sistema operando normalmente. Data/Hora: {DateTime.UtcNow}"),
                Cron.Minutely);

            return app;
        }
    }
}