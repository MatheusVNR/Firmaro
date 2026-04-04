using Firmaro.Application.Interfaces.Repositories;
using Firmaro.Application.Interfaces.Services;
using Firmaro.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firmaro.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IAppointmentRepository appointmentRepository,
                                   ILogger<NotificationService> logger)
        {
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }

        public async Task SendReminderAsync(Guid appointmentId)
        {
            Appointment? appointment = await _appointmentRepository.GetByIdForSystemAsync(appointmentId);
            if (appointment == null)
            {
                _logger.LogWarning($"[Notification] Job abortado. Agendamento {appointmentId} não encontrado.");
                return;
            }

            _logger.LogInformation("=================================================");
            _logger.LogInformation("PLANNING: DISPARO DE NOTIFICAÇÃO");
            _logger.LogInformation($"PARA: {appointment.ClientName} ({appointment.ClientPhone})");
            _logger.LogInformation($"DATA/HORA: {appointment.DateTime}");
            _logger.LogInformation($"TOKEN DE CONFIRMAÇÃO: {appointment.ConfirmationToken}");
            _logger.LogInformation("=================================================");

            // ainda no campo das ideias, provável plano pra dps:

            /* 
             * 1. Injetar IWhatsAppProvider ou IEmailProvider.
             * 2. Montar o template da mensagem com o link de confirmação: "Olá {name}, confirme seu horário em: firmaro.com/confirm/{token}"
             * 3. Chamar o provedor real.
             * 4. Atualizar a tabela 'notifications' com o status 'Sent'.
            */

            await Task.CompletedTask;
        }
    }
}
