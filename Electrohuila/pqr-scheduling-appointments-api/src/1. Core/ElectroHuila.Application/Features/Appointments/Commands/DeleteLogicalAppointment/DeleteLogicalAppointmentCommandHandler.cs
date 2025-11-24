using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.DeleteLogicalAppointment;

/// <summary>
/// Manejador del comando para eliminación lógica de citas
/// </summary>
public class DeleteLogicalAppointmentCommandHandler : IRequestHandler<DeleteLogicalAppointmentCommand, Result>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public DeleteLogicalAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result> Handle(DeleteLogicalAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.Id);
            if (appointment == null)
            {
                return Result.Failure($"Cita con ID {request.Id} no encontrada");
            }

            if (!appointment.IsActive)
            {
                return Result.Failure($"La cita con ID {request.Id} ya está inactiva");
            }

            if (!appointment.IsEnabled)
            {
                return Result.Failure($"La cita con ID {request.Id} ya está deshabilitada");
            }

            // Eliminación lógica (ambos campos)
            appointment.IsActive = false;
            appointment.IsEnabled = false;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _appointmentRepository.UpdateAsync(appointment);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error al eliminar lógicamente la cita: {ex.Message}");
        }
    }
}
