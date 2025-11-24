using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Commands.CreateLocalHoliday;

/// <summary>
/// Comando para crear un festivo local (específico de una sucursal)
/// </summary>
public record CreateLocalHolidayCommand(CreateLocalHolidayDto Dto) : IRequest<Result<HolidayDto>>;
