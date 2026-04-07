using Firmaro.Application.Interfaces.Repositories;
using Firmaro.Domain.Entities;
using Firmaro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmaro.Infrastructure.Repositories
{
    public class AutomationSettingsRepository : IAutomationSettingsRepository
    {
        private readonly FirmaroDbContext _context;

        public AutomationSettingsRepository(FirmaroDbContext context)
        {
            _context = context;
        }


        public async Task<AutomationSettings?> GetByUserIdAsync(Guid userId)
        {
            return await _context.AutomationSettings.FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task AddAsync(AutomationSettings settings)
        {
            await _context.AutomationSettings.AddAsync(settings);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AutomationSettings settings)
        {
            _context.AutomationSettings.Update(settings);
            await _context.SaveChangesAsync();
        }
    }
}
