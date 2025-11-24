using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Queries.CheckIfHoliday;

/// <summary>
/// Manejador de query para verificar si una fecha es festivo
/// </summary>
public class CheckIfHolidayQueryHandler : IRequestHandler<CheckIfHolidayQuery, Result<HolidayCheckResultDto>>
{
    private readonly IHolidayRepository _holidayRepository;

    public CheckIfHolidayQueryHandler(IHolidayRepository holidayRepository)
    {
        _holidayRepository = holidayRepository;
    }

    public async Task<Result<HolidayCheckResultDto>> Handle(CheckIfHolidayQuery request, CancellationToken cancellationToken)
    {
        var holidays = await _holidayRepository.GetByDateAsync(request.Date, cancellationToken);

        // Filtrar festivos que aplican a la sucursal especificada
        var applicableHoliday = holidays
            .Where(h => h.IsActive)
            .FirstOrDefault(h => h.AppliesToBranch(request.BranchId));

        if (applicableHoliday != null)
        {
            var result = new HolidayCheckResultDto
            {
                IsHoliday = true,
                HolidayName = applicableHoliday.HolidayName,
                HolidayType = applicableHoliday.HolidayType
            };
            return Result<HolidayCheckResultDto>.Success(result);
        }

        var notHolidayResult = new HolidayCheckResultDto
        {
            IsHoliday = false,
            HolidayName = null,
            HolidayType = null
        };
        return Result<HolidayCheckResultDto>.Success(notHolidayResult);
    }
}
