using Firmaro.Application.DTOs;
using Firmaro.Application.Interfaces.Repositories;
using Firmaro.Application.Interfaces.Services;
using Firmaro.Domain.Entities;
using Firmaro.Domain.Enums;

namespace Firmaro.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;
        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _repository = appointmentRepository;
        }


        public async Task<AppointmentResponse> CreateAsync(Guid userId, CreateAppointmentRequest request)
        {
            if (request.DateTime < DateTime.UtcNow)
                throw new ArgumentException("A data do agendamento não pode estar no passado."); // conferir sobre traduções dps

            Appointment appointment = new()
            {
                UserId = userId,
                ClientName = request.ClientName,
                ClientPhone = request.ClientPhone,
                DateTime = request.DateTime,
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(appointment);

            return new AppointmentResponse(
                appointment.Id,
                appointment.ClientName,
                appointment.ClientPhone,
                appointment.DateTime,
                appointment.Status.ToString());
        }

        public async Task<IEnumerable<AppointmentResponse>> GetAllAsync(Guid userId)
        {
            IEnumerable<Appointment> appointments = await _repository.GetAllByUserIdAsync(userId);

            return appointments
                .Select(a => new AppointmentResponse(a.Id, a.ClientName, a.ClientPhone, a.DateTime, a.Status.ToString()));
        }

        public async Task<AppointmentResponse> GetByIdAsync(Guid id, Guid userId)
        {
            Appointment appointment = await _repository.GetByIdAsync(id, userId)
                ?? throw new KeyNotFoundException("Agendamento não encontrado.");

            return new AppointmentResponse(
                appointment.Id, 
                appointment.ClientName, 
                appointment.ClientPhone, 
                appointment.DateTime, 
                appointment.Status.ToString());
        }

        public async Task UpdateAsync(Guid id, Guid userId, UpdateAppointmentRequest request)
        {
            Appointment appointment = await _repository.GetByIdAsync(id, userId)
                ?? throw new KeyNotFoundException("Agendamento não encontrado.");

            appointment.ClientName = request.ClientName;
            appointment.ClientPhone = request.ClientPhone;
            appointment.DateTime = request.DateTime;

            await _repository.UpdateAsync(appointment);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            Appointment appointment = await _repository.GetByIdAsync(id, userId)
                ?? throw new KeyNotFoundException("Agendamento não encontrado.");

            await _repository.DeleteAsync(appointment);
        }
    }
}
