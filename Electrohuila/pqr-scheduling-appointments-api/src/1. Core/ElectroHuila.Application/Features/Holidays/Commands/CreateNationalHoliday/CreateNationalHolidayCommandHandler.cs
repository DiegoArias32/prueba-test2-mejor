using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Catalogs;
using ElectroHuila.Domain.Entities.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Commands.CreateNationalHoliday;

/// <summary>
/// Manejador del comando para crear un festivo nacional
/// </summary>
public class CreateNationalHolidayCommandHandler : IRequestHandler<CreateNationalHolidayCommand, Result<HolidayDto>>
{
    private readonly IHolidayRepository _holidayRepository;
    private readonly IMapper _mapper;

    public CreateNationalHolidayCommandHandler(
        IHolidayRepository holidayRepository,
        IMapper mapper)
    {
        _holidayRepository = holidayRepository;
        _mapper = mapper;
    }

    public async Task<Result<HolidayDto>> Handle(CreateNationalHolidayCommand request, CancellationToken cancellationToken)
    {
        // Validar que la fecha no esté en el pasado
        if (request.Dto.HolidayDate.Date < DateTime.UtcNow.Date)
        {
            return Result.Failure<HolidayDto>("No se puede crear un festivo en el pasado");
        }

        // Verificar si ya existe un festivo en esa fecha
        var existingHolidays = await _holidayRepository.GetByDateAsync(request.Dto.HolidayDate, cancellationToken);
        if (existingHolidays.Any(h => h.HolidayType == "NATIONAL"))
        {
            return Result.Failure<HolidayDto>("Ya existe un festivo nacional en esa fecha");
        }

        // Crear el festivo nacional
        var holiday = Holiday.CreateNationalHoliday(
            request.Dto.HolidayDate,
            request.Dto.HolidayName
        );

        await _holidayRepository.AddAsync(holiday);

        var dto = _mapper.Map<HolidayDto>(holiday);
        return Result.Success(dto);
    }
}
