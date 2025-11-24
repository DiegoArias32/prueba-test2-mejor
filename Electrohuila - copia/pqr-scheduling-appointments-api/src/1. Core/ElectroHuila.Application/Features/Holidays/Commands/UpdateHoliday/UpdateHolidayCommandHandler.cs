using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Commands.UpdateHoliday;

/// <summary>
/// Manejador del comando para actualizar un festivo
/// </summary>
public class UpdateHolidayCommandHandler : IRequestHandler<UpdateHolidayCommand, Result<HolidayDto>>
{
    private readonly IHolidayRepository _holidayRepository;
    private readonly IMapper _mapper;

    public UpdateHolidayCommandHandler(
        IHolidayRepository holidayRepository,
        IMapper mapper)
    {
        _holidayRepository = holidayRepository;
        _mapper = mapper;
    }

    public async Task<Result<HolidayDto>> Handle(UpdateHolidayCommand request, CancellationToken cancellationToken)
    {
        // Buscar el festivo
        var holiday = await _holidayRepository.GetByIdAsync(request.Dto.Id);
        if (holiday == null)
        {
            return Result.Failure<HolidayDto>($"Festivo con ID {request.Dto.Id} no encontrado");
        }

        // Actualizar nombre
        if (!string.IsNullOrWhiteSpace(request.Dto.HolidayName))
        {
            holiday.UpdateDetails(request.Dto.HolidayName);
        }

        // Actualizar fecha si se proporciona
        if (request.Dto.HolidayDate.HasValue)
        {
            if (request.Dto.HolidayDate.Value.Date < DateTime.UtcNow.Date)
            {
                return Result.Failure<HolidayDto>("No se puede establecer una fecha en el pasado");
            }

            holiday.UpdateDate(request.Dto.HolidayDate.Value);
        }

        await _holidayRepository.UpdateAsync(holiday);

        var dto = _mapper.Map<HolidayDto>(holiday);
        return Result.Success(dto);
    }
}
