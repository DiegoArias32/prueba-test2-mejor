using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Holidays.Queries.GetAllHolidays;

/// <summary>
/// Manejador de query para obtener todos los festivos con paginación
/// </summary>
public class GetAllHolidaysQueryHandler : IRequestHandler<GetAllHolidaysQuery, Result<PagedResult<HolidayDto>>>
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

    public async Task<Result<PagedResult<HolidayDto>>> Handle(GetAllHolidaysQuery request, CancellationToken cancellationToken)
    {
        // Obtener festivos paginados
        var holidays = await _holidayRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        // Obtener total de registros
        var totalCount = await _holidayRepository.CountAsync(cancellationToken);

        // Mapear a DTOs
        var holidayDtos = _mapper.Map<List<HolidayDto>>(holidays);

        // Crear resultado paginado
        var pagedResult = new PagedResult<HolidayDto>
        {
            Items = holidayDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return Result.Success(pagedResult);
    }
}
