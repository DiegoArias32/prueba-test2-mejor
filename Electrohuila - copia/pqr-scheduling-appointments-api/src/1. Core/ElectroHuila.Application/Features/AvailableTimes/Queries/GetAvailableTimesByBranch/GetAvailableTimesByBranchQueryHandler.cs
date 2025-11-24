using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.AvailableTimes;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Queries.GetAvailableTimesByBranch;

public class GetAvailableTimesByBranchQueryHandler : IRequestHandler<GetAvailableTimesByBranchQuery, Result<IEnumerable<AvailableTimeDto>>>
{
    private readonly IAvailableTimeRepository _availableTimeRepository;
    private readonly IMapper _mapper;

    public GetAvailableTimesByBranchQueryHandler(IAvailableTimeRepository availableTimeRepository, IMapper mapper)
    {
        _availableTimeRepository = availableTimeRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AvailableTimeDto>>> Handle(GetAvailableTimesByBranchQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var availableTimes = await _availableTimeRepository.GetByBranchIdAsync(request.BranchId);
            var availableTimeDtos = _mapper.Map<IEnumerable<AvailableTimeDto>>(availableTimes);

            return Result.Success(availableTimeDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<AvailableTimeDto>>($"Error retrieving available times by branch: {ex.Message}");
        }
    }
}