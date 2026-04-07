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
        private readonly IAutomationSettingsService _settingsService;

        public AppointmentService(IAppointmentRepository appointmentRepository,
                                  IJobScheduler jobScheduler,
                                  IAutomationSettingsService automationSettingsService)
        {
            _repository = appointmentRepository;
            _jobScheduler = jobScheduler;
            _settingsService = automationSettingsService;
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
            await ScheduleRemindersAsync(appointment);

            return new AppointmentResponse(
                appointment.Id,
                appointment.ClientName,
                appointment.ClientPhone,
                appointment.DateTime,
                appointment.Status.ToString());
        }

        private async Task ScheduleRemindersAsync(Appointment appointment)
        {
            AutomationSettings settings = await _settingsService.GetSettingsAsync(appointment.UserId);

            if (settings.SendDayBefore)
            {
                DateTimeOffset dayBefore = appointment.DateTime.AddDays(-1);
                _jobScheduler.ScheduleReminder(appointment.Id, dayBefore);
            }

            if (settings.SendHoursBefore > 0)
            {
                DateTimeOffset hoursBefore = appointment.DateTime.AddHours(-settings.SendHoursBefore);
                _jobScheduler.ScheduleReminder(appointment.Id, hoursBefore);
            }
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
