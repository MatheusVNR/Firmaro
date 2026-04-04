using Firmaro.Application.Interfaces.Repositories;
using Firmaro.Domain.Entities;
using Firmaro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmaro.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly FirmaroDbContext _context;
        public AppointmentRepository(FirmaroDbContext context)
        {
            _context = context;
        }


        public async Task<Appointment?> GetByIdAsync(Guid id, Guid userId)
        {
            return await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        }

        public async Task<Appointment?> GetByIdForSystemAsync(Guid id)
        {
            return await _context.Appointments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Appointment?> GetByConfirmationTokenAsync(string token)
        {
            return await _context.Appointments.FirstOrDefaultAsync(f => f.ConfirmationToken == token);
        }

        public async Task<IEnumerable<Appointment>> GetAllByUserIdAsync(Guid userId)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
        }
            
        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }
}
