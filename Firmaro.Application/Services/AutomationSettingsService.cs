using Firmaro.Application.DTOs;
using Firmaro.Application.Interfaces.Repositories;
using Firmaro.Application.Interfaces.Services;
using Firmaro.Domain.Entities;

namespace Firmaro.Application.Services
{
    public class AutomationSettingsService : IAutomationSettingsService
    {
        private readonly IAutomationSettingsRepository _repository;

        public AutomationSettingsService(IAutomationSettingsRepository repository)
        {
            _repository = repository;
        }


        public async Task CreateDefaultSettingsAsync(Guid userId)
        {
            AutomationSettings? existing = await _repository.GetByUserIdAsync(userId);
            if (existing is not null) return;

            AutomationSettings defaultSettings = new()
            {
                UserId = userId,
                SendDayBefore = true,
                SendHoursBefore = 2,
                RequireConfirmation = true
            };

            await _repository.AddAsync(defaultSettings);
        }

        public async Task<AutomationSettings> GetSettingsAsync(Guid userId)
        {
            AutomationSettings? settings = await _repository.GetByUserIdAsync(userId);

            if (settings is null)
            {
                await CreateDefaultSettingsAsync(userId);

                return await _repository.GetByUserIdAsync(userId) 
                    ?? throw new Exception("Falha ao gerar configurações.");
            }

            return settings;
        }

        public async Task UpdateSettingsAsync(Guid userId, UpdateAutomationSettingsRequest request)
        {
            AutomationSettings settings = await GetSettingsAsync(userId);

            settings.SendDayBefore = request.SendDayBefore;
            settings.SendHoursBefore = request.SendHoursBefore;
            settings.RequireConfirmation = request.RequireConfirmation;

            await _repository.UpdateAsync(settings);
        }
    }
}
