using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Commands.CreateNationalHoliday;

/// <summary>
/// Comando para crear un festivo nacional
/// </summary>
public record CreateNationalHolidayCommand(CreateNationalHolidayDto Dto) : IRequest<Result<HolidayDto>>;
