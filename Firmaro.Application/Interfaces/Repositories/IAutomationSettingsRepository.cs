using Firmaro.Domain.Entities;

namespace Firmaro.Application.Interfaces.Repositories
{
    public interface IAutomationSettingsRepository
    {
        Task<AutomationSettings?> GetByUserIdAsync(Guid userId);
        Task AddAsync(AutomationSettings settings);
        Task UpdateAsync(AutomationSettings settings);
    }
}
