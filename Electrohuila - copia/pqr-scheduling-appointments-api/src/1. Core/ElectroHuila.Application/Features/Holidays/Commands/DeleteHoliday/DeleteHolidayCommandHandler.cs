using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Commands.DeleteHoliday;

/// <summary>
/// Manejador del comando para eliminar un festivo
/// </summary>
public class DeleteHolidayCommandHandler : IRequestHandler<DeleteHolidayCommand, Result<bool>>
{
    private readonly IHolidayRepository _holidayRepository;

    public DeleteHolidayCommandHandler(IHolidayRepository holidayRepository)
    {
        _holidayRepository = holidayRepository;
    }

    public async Task<Result<bool>> Handle(DeleteHolidayCommand request, CancellationToken cancellationToken)
    {
        // Buscar el festivo
        var holiday = await _holidayRepository.GetByIdAsync(request.Id);
        if (holiday == null)
        {
            return Result.Failure<bool>($"Festivo con ID {request.Id} no encontrado");
        }

        // Desactivar el festivo (soft delete)
        holiday.IsActive = false;
        holiday.UpdatedAt = DateTime.UtcNow;
        await _holidayRepository.UpdateAsync(holiday);

        return Result.Success(true);
    }
}
