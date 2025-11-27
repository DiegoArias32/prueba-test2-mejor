using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Commands.ActivateAppointmentType;

/// <summary>
/// Manejador del comando para activar tipos de cita previamente desactivados
/// </summary>
public class ActivateAppointmentTypeCommandHandler : IRequestHandler<ActivateAppointmentTypeCommand, Result>
{
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;

    public ActivateAppointmentTypeCommandHandler(IAppointmentTypeRepository appointmentTypeRepository)
    {
        _appointmentTypeRepository = appointmentTypeRepository;
    }

    public async Task<Result> Handle(ActivateAppointmentTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointmentType = await _appointmentTypeRepository.GetByIdAsync(request.Id);
            if (appointmentType == null)
            {
                return Result.Failure($"Tipo de cita con ID {request.Id} no encontrado");
            }

            if (appointmentType.IsActive)
            {
                return Result.Failure($"El tipo de cita con ID {request.Id} ya está activo");
            }

            // Activación
            appointmentType.IsActive = true;
            appointmentType.UpdatedAt = DateTime.UtcNow;
            await _appointmentTypeRepository.UpdateAsync(appointmentType);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error al activar el tipo de cita: {ex.Message}");
        }
    }
}
