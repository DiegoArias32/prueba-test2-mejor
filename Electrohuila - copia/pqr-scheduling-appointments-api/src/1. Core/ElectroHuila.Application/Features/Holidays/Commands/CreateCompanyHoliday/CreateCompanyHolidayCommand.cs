using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Commands.CreateCompanyHoliday;

/// <summary>
/// Comando para crear un festivo de empresa (aplica a todas las sucursales)
/// </summary>
public record CreateCompanyHolidayCommand(CreateCompanyHolidayDto Dto) : IRequest<Result<HolidayDto>>;
