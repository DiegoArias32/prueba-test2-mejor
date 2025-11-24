using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Queries.CheckIfHoliday;

/// <summary>
/// Query para verificar si una fecha específica es un festivo
/// </summary>
public record CheckIfHolidayQuery(DateTime Date, int? BranchId = null) : IRequest<Result<HolidayCheckResultDto>>;
