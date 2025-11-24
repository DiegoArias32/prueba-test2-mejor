using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Queries.GetHolidaysByDateRange;

/// <summary>
/// Query para obtener festivos dentro de un rango de fechas
/// </summary>
public record GetHolidaysByDateRangeQuery(DateTime StartDate, DateTime EndDate) : IRequest<Result<IEnumerable<HolidayDto>>>;
