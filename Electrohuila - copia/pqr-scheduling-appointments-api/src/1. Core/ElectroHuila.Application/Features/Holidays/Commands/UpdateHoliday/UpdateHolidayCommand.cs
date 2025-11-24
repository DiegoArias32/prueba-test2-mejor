using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Commands.UpdateHoliday;

/// <summary>
/// Comando para actualizar un festivo existente
/// </summary>
public record UpdateHolidayCommand(UpdateHolidayDto Dto) : IRequest<Result<HolidayDto>>;
