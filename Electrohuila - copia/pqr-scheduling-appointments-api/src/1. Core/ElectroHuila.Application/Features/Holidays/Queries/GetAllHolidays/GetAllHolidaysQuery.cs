using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Queries.GetAllHolidays;

/// <summary>
/// Query para obtener todos los festivos
/// </summary>
public record GetAllHolidaysQuery : IRequest<Result<IEnumerable<HolidayDto>>>;
