using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Commands.DeleteLogicalAppointmentType;

/// <summary>
/// Manejador del comando para eliminación lógica de tipos de cita
/// </summary>
public class DeleteLogicalAppointmentTypeCommandHandler : IRequestHandler<DeleteLogicalAppointmentTypeCommand, Result>
{
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;

    public DeleteLogicalAppointmentTypeCommandHandler(IAppointmentTypeRepository appointmentTypeRepository)
    {
        _appointmentTypeRepository = appointmentTypeRepository;
    }

    public async Task<Result> Handle(DeleteLogicalAppointmentTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointmentType = await _appointmentTypeRepository.GetByIdAsync(request.Id);
            if (appointmentType == null)
            {
                return Result.Failure($"Tipo de cita con ID {request.Id} no encontrado");
            }

            if (!appointmentType.IsActive)
            {
                return Result.Failure($"El tipo de cita con ID {request.Id} ya está inactivo");
            }

            // Eliminación lógica
            appointmentType.IsActive = false;
            appointmentType.UpdatedAt = DateTime.UtcNow;
            await _appointmentTypeRepository.UpdateAsync(appointmentType);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error al eliminar lógicamente el tipo de cita: {ex.Message}");
        }
    }
}
