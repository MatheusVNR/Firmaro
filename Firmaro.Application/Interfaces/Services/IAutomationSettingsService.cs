using Firmaro.Application.DTOs;
using Firmaro.Domain.Entities;

namespace Firmaro.Application.Interfaces.Services
{
    public interface IAutomationSettingsService
    {
        Task CreateDefaultSettingsAsync(Guid userId);
        Task<AutomationSettings> GetSettingsAsync(Guid userId);
        Task UpdateSettingsAsync(Guid userId, UpdateAutomationSettingsRequest request);
    }
}
