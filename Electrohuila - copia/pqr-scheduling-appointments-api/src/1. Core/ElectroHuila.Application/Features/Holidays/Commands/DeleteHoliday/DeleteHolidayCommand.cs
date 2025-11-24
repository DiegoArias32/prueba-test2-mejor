using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Commands.DeleteHoliday;

/// <summary>
/// Comando para eliminar (desactivar) un festivo
/// </summary>
public record DeleteHolidayCommand(int Id) : IRequest<Result<bool>>;
