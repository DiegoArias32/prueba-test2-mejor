using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Queries.GetAllHolidays;

/// <summary>
/// Manejador de query para obtener todos los festivos
/// </summary>
public class GetAllHolidaysQueryHandler : IRequestHandler<GetAllHolidaysQuery, Result<IEnumerable<HolidayDto>>>
{
    private readonly IHolidayRepository _holidayRepository;
    private readonly IMapper _mapper;

    public GetAllHolidaysQueryHandler(
        IHolidayRepository holidayRepository,
        IMapper mapper)
    {
        _holidayRepository = holidayRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<HolidayDto>>> Handle(GetAllHolidaysQuery request, CancellationToken cancellationToken)
    {
        var holidays = await _holidayRepository.GetAllAsync();
        var holidayDtos = _mapper.Map<IEnumerable<HolidayDto>>(holidays);

        return Result.Success(holidayDtos);
    }
}
