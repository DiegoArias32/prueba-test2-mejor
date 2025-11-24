using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.AvailableTimes;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Queries.GetConfiguredTimesByBranch;

public class GetConfiguredTimesByBranchQueryHandler : IRequestHandler<GetConfiguredTimesByBranchQuery, Result<IEnumerable<AvailableTimeDto>>>
{
    private readonly IAvailableTimeRepository _availableTimeRepository;
    private readonly IMapper _mapper;

    public GetConfiguredTimesByBranchQueryHandler(IAvailableTimeRepository availableTimeRepository, IMapper mapper)
    {
        _availableTimeRepository = availableTimeRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AvailableTimeDto>>> Handle(GetConfiguredTimesByBranchQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var times = await _availableTimeRepository.GetByBranchIdAsync(request.BranchId);
            var timeDtos = _mapper.Map<IEnumerable<AvailableTimeDto>>(times);

            return Result.Success(timeDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<AvailableTimeDto>>($"Error al obtener horas configuradas: {ex.Message}");
        }
    }
}
