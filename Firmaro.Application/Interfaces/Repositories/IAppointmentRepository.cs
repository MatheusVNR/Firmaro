using Firmaro.Domain.Entities;

namespace Firmaro.Application.Interfaces.Repositories
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetByIdAsync(Guid id, Guid userId);
        Task<Appointment?> GetByIdForSystemAsync(Guid id);
        Task<Appointment?> GetByConfirmationTokenAsync(string token);
        Task<IEnumerable<Appointment>> GetAllByUserIdAsync(Guid userId);
        Task AddAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task DeleteAsync(Appointment appointment);
    }
}
