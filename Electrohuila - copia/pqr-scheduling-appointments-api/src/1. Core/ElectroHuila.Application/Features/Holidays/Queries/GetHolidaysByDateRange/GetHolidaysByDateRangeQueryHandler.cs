using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Queries.GetHolidaysByDateRange;

/// <summary>
/// Manejador de query para obtener festivos por rango de fechas
/// </summary>
public class GetHolidaysByDateRangeQueryHandler : IRequestHandler<GetHolidaysByDateRangeQuery, Result<IEnumerable<HolidayDto>>>
{
    private readonly IHolidayRepository _holidayRepository;
    private readonly IMapper _mapper;

    public GetHolidaysByDateRangeQueryHandler(
        IHolidayRepository holidayRepository,
        IMapper mapper)
    {
        _holidayRepository = holidayRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<HolidayDto>>> Handle(GetHolidaysByDateRangeQuery request, CancellationToken cancellationToken)
    {
        if (request.StartDate > request.EndDate)
        {
            return Result.Failure<IEnumerable<HolidayDto>>("La fecha de inicio debe ser anterior a la fecha de fin");
        }

        var holidays = await _holidayRepository.GetByDateRangeAsync(request.StartDate, request.EndDate, null, cancellationToken);
        var holidayDtos = _mapper.Map<IEnumerable<HolidayDto>>(holidays);

        return Result.Success(holidayDtos);
    }
}
