using Firmaro.Application.DTOs;

namespace Firmaro.Application.Interfaces.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentResponse> CreateAsync(Guid userId, CreateAppointmentRequest request);
        Task<IEnumerable<AppointmentResponse>> GetAllAsync(Guid userId);
        Task<AppointmentResponse> GetByIdAsync(Guid id, Guid userId);
        Task UpdateAsync(Guid id, Guid userId, UpdateAppointmentRequest request);
        Task DeleteAsync(Guid id, Guid userId);
        Task ConfirmByTokenAsync(string token);
        Task CancelByTokenAsync(string token);
    }
}
