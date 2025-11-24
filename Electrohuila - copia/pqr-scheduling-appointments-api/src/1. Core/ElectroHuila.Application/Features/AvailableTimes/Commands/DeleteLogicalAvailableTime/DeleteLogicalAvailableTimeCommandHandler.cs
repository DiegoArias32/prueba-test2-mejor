using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Commands.DeleteLogicalAvailableTime;

/// <summary>
/// Manejador del comando para eliminación lógica de horarios disponibles
/// </summary>
public class DeleteLogicalAvailableTimeCommandHandler : IRequestHandler<DeleteLogicalAvailableTimeCommand, Result>
{
    private readonly IAvailableTimeRepository _availableTimeRepository;

    public DeleteLogicalAvailableTimeCommandHandler(IAvailableTimeRepository availableTimeRepository)
    {
        _availableTimeRepository = availableTimeRepository;
    }

    public async Task<Result> Handle(DeleteLogicalAvailableTimeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var availableTime = await _availableTimeRepository.GetByIdAsync(request.Id);
            if (availableTime == null)
            {
                return Result.Failure($"Horario disponible con ID {request.Id} no encontrado");
            }

            if (!availableTime.IsActive)
            {
                return Result.Failure($"El horario disponible con ID {request.Id} ya está inactivo");
            }

            // Eliminación lógica
            availableTime.IsActive = false;
            availableTime.UpdatedAt = DateTime.UtcNow;
            await _availableTimeRepository.UpdateAsync(availableTime);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error al eliminar lógicamente el horario disponible: {ex.Message}");
        }
    }
}
