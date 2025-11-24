using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Queries.GetHolidayById;

/// <summary>
/// Query para obtener un festivo por su ID
/// </summary>
public record GetHolidayByIdQuery(int Id) : IRequest<Result<HolidayDto>>;
