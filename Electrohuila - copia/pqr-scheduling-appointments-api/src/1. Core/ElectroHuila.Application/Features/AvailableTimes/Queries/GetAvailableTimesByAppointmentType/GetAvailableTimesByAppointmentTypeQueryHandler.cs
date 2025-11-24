using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.AvailableTimes;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Queries.GetAvailableTimesByAppointmentType;

public class GetAvailableTimesByAppointmentTypeQueryHandler : IRequestHandler<GetAvailableTimesByAppointmentTypeQuery, Result<IEnumerable<AvailableTimeDto>>>
{
    private readonly IAvailableTimeRepository _availableTimeRepository;
    private readonly IMapper _mapper;

    public GetAvailableTimesByAppointmentTypeQueryHandler(IAvailableTimeRepository availableTimeRepository, IMapper mapper)
    {
        _availableTimeRepository = availableTimeRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AvailableTimeDto>>> Handle(GetAvailableTimesByAppointmentTypeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var availableTimes = await _availableTimeRepository.GetByAppointmentTypeIdAsync(request.AppointmentTypeId);
            var availableTimeDtos = _mapper.Map<IEnumerable<AvailableTimeDto>>(availableTimes);

            return Result.Success(availableTimeDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<AvailableTimeDto>>($"Error retrieving available times by appointment type: {ex.Message}");
        }
    }
}