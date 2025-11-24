using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Queries.GetHolidayById;

/// <summary>
/// Manejador de query para obtener un festivo por ID
/// </summary>
public class GetHolidayByIdQueryHandler : IRequestHandler<GetHolidayByIdQuery, Result<HolidayDto>>
{
    private readonly IHolidayRepository _holidayRepository;
    private readonly IMapper _mapper;

    public GetHolidayByIdQueryHandler(
        IHolidayRepository holidayRepository,
        IMapper mapper)
    {
        _holidayRepository = holidayRepository;
        _mapper = mapper;
    }

    public async Task<Result<HolidayDto>> Handle(GetHolidayByIdQuery request, CancellationToken cancellationToken)
    {
        var holiday = await _holidayRepository.GetByIdAsync(request.Id);

        if (holiday == null)
        {
            return Result.Failure<HolidayDto>($"Festivo con ID {request.Id} no encontrado");
        }

        var holidayDto = _mapper.Map<HolidayDto>(holiday);
        return Result.Success(holidayDto);
    }
}
