using Firmaro.Application.DTOs;
using Firmaro.Application.Interfaces.Repositories;
using Firmaro.Application.Interfaces.Services;
using Firmaro.Domain.Entities;
using Firmaro.Domain.Enums;
using System.Security.Cryptography;

namespace Firmaro.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;
        private readonly IJobScheduler _jobScheduler;
        public AppointmentService(IAppointmentRepository appointmentRepository,
                                  IJobScheduler jobScheduler)
        {
            _repository = appointmentRepository;
            _jobScheduler = jobScheduler;
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
                ConfirmationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLower(),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(appointment);

            // No futuro, só pegar esse tempo da AutomationSettings
            // porém, por enquanto, 24h antes do evento tá bão
            DateTimeOffset reminderTime = appointment.DateTime.AddHours(-24);

            // se essa data já passou, envia logo daqui a 1 minuto.
            if (reminderTime < DateTime.UtcNow)
                reminderTime = DateTime.UtcNow.AddMinutes(1);

            _jobScheduler.ScheduleReminder(appointment.Id, reminderTime);

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

        public async Task ConfirmByTokenAsync(string token)
        {
            // lembrar de trocar de exceptions para padrão Result depois

            Appointment? appointment = await _repository.GetByConfirmationTokenAsync(token)
                ?? throw new KeyNotFoundException("Link de confirmação inválido ou expirado.");

            if (appointment.Status != AppointmentStatus.Pending)
                throw new InvalidOperationException($"O agendamento não pode ser confirmado pois está: {appointment.Status}");

            appointment.Status = AppointmentStatus.Confirmed;
            await _repository.UpdateAsync(appointment);
        }

        public async Task CancelByTokenAsync(string token)
        {
            Appointment? appointment = await _repository.GetByConfirmationTokenAsync(token)
                ?? throw new KeyNotFoundException("Link de cancelamento inválido ou expirado.");

            if (appointment.Status == AppointmentStatus.Cancelled)
                return;

            appointment.Status = AppointmentStatus.Cancelled;
            await _repository.UpdateAsync(appointment);
        }
    }
}
