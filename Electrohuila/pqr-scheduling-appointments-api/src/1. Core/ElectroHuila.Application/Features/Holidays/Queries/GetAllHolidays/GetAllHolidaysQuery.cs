using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Queries.GetAllHolidays;

/// <summary>
/// Query para obtener todos los festivos con paginación
/// </summary>
public record GetAllHolidaysQuery : IRequest<Result<PagedResult<HolidayDto>>>
{
    /// <summary>
    /// Número de página a obtener (basado en 1)
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Cantidad de registros por página
    /// </summary>
    public int PageSize { get; init; } = 20;
}
