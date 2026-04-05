using Firmaro.Api.Extensions;
using Firmaro.Application;
using Firmaro.Infrastructure;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Documentação nativa
builder.Services.AddNativeOpenApi();

builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Mapeia o JSON gerado pela Microsoft
    app.MapScalarApiReference(); // Mapeia a interface visual do Scalar
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboardConfiguration(app.Configuration);
app.MapControllers();
app.Run();