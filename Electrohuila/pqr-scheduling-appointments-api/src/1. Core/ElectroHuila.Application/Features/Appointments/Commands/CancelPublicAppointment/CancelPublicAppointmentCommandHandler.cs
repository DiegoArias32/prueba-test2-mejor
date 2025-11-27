using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Contracts.Services;
using ElectroHuila.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Application.Features.Appointments.Commands.CancelPublicAppointment;

public class CancelPublicAppointmentCommandHandler : IRequestHandler<CancelPublicAppointmentCommand, Result<bool>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IClientRepository _clientRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CancelPublicAppointmentCommandHandler> _logger;

    public CancelPublicAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IClientRepository clientRepository,
        INotificationService notificationService,
        ILogger<CancelPublicAppointmentCommandHandler> logger)
    {
        _appointmentRepository = appointmentRepository;
        _clientRepository = clientRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(CancelPublicAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ClientNumber))
                return Result.Failure<bool>("El número de cliente es requerido");

            if (string.IsNullOrWhiteSpace(request.Reason))
                return Result.Failure<bool>("El motivo de cancelación es requerido");

            // Verificar que el cliente existe (sin cargar relaciones para evitar tracking conflicts)
            var clientId = await _clientRepository.GetClientIdByNumberAsync(request.ClientNumber);
            if (clientId == null)
                return Result.Failure<bool>("Cliente no encontrado");

            // Obtener la cita (GetByIdAsync usa AsNoTracking)
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
                return Result.Failure<bool>("Cita no encontrada");

            // Verificar que la cita pertenece al cliente
            if (appointment.ClientId != clientId.Value)
                return Result.Failure<bool>("La cita no pertenece a este cliente");

            // StatusId: 5=CANCELLED
            const int CANCELLED_STATUS_ID = 5;

            // Update appointment status
            appointment.StatusId = CANCELLED_STATUS_ID;
            appointment.CancellationReason = request.Reason;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);

            // Enviar notificación de cancelación (Email y WhatsApp)
            // Si falla la notificación, no afecta la cancelación de la cita
            try
            {
                _logger.LogInformation(
                    "Enviando notificación de cancelación para cita {AppointmentId}",
                    request.AppointmentId);

                await _notificationService.SendAppointmentCancellationAsync(
                    request.AppointmentId,
                    request.Reason,
                    cancellationToken);

                _logger.LogInformation(
                    "Notificación de cancelación enviada exitosamente para cita {AppointmentId}",
                    request.AppointmentId);
            }
            catch (Exception notificationEx)
            {
                // Log error but don't fail the cancellation
                _logger.LogError(notificationEx,
                    "Error al enviar notificación de cancelación para cita {AppointmentId}. La cita fue cancelada correctamente.",
                    request.AppointmentId);
            }

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error al cancelar cita: {ex.Message}");
        }
    }
}
