using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Contracts.Services;
using ElectroHuila.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Application.Features.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Result>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CancelAppointmentCommandHandler> _logger;

    public CancelAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        INotificationService notificationService,
        ILogger<CancelAppointmentCommandHandler> logger)
    {
        _appointmentRepository = appointmentRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.Id);
            if (appointment == null)
            {
                return Result.Failure("Appointment not found");
            }

            // StatusIds: 4=COMPLETED, 5=CANCELLED
            const int COMPLETED_STATUS_ID = 4;
            const int CANCELLED_STATUS_ID = 5;

            if (appointment.StatusId == CANCELLED_STATUS_ID)
            {
                return Result.Failure("Appointment is already cancelled");
            }

            if (appointment.StatusId == COMPLETED_STATUS_ID)
            {
                return Result.Failure("Cannot cancel a completed appointment");
            }

            appointment.StatusId = CANCELLED_STATUS_ID;
            appointment.CancellationReason = request.CancellationReason;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);

            // Enviar notificación de cancelación (Email y WhatsApp)
            // Si falla la notificación, no afecta la cancelación de la cita
            try
            {
                _logger.LogInformation(
                    "Enviando notificación de cancelación para cita {AppointmentId}",
                    request.Id);

                await _notificationService.SendAppointmentCancellationAsync(
                    request.Id,
                    request.CancellationReason ?? "No especificado",
                    cancellationToken);

                _logger.LogInformation(
                    "Notificación de cancelación enviada exitosamente para cita {AppointmentId}",
                    request.Id);
            }
            catch (Exception notificationEx)
            {
                // Log error but don't fail the cancellation
                _logger.LogError(notificationEx,
                    "Error al enviar notificación de cancelación para cita {AppointmentId}. La cita fue cancelada correctamente.",
                    request.Id);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error cancelling appointment: {ex.Message}");
        }
    }
}