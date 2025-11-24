using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.AvailableTimes;
using ElectroHuila.Domain.Entities.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Commands.BulkCreateAvailableTimes;

public class BulkCreateAvailableTimesCommandHandler : IRequestHandler<BulkCreateAvailableTimesCommand, Result<List<AvailableTimeDto>>>
{
    private readonly IAvailableTimeRepository _availableTimeRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IMapper _mapper;

    public BulkCreateAvailableTimesCommandHandler(
        IAvailableTimeRepository availableTimeRepository,
        IBranchRepository branchRepository,
        IAppointmentTypeRepository appointmentTypeRepository,
        IMapper mapper)
    {
        _availableTimeRepository = availableTimeRepository;
        _branchRepository = branchRepository;
        _appointmentTypeRepository = appointmentTypeRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<AvailableTimeDto>>> Handle(BulkCreateAvailableTimesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var branchExists = await _branchRepository.ExistsAsync(request.Dto.BranchId);
            if (!branchExists)
            {
                return Result.Failure<List<AvailableTimeDto>>($"Branch with ID {request.Dto.BranchId} not found");
            }

            if (request.Dto.AppointmentTypeId.HasValue)
            {
                var appointmentTypeExists = await _appointmentTypeRepository.ExistsAsync(request.Dto.AppointmentTypeId.Value);
                if (!appointmentTypeExists)
                {
                    return Result.Failure<List<AvailableTimeDto>>($"Appointment type with ID {request.Dto.AppointmentTypeId.Value} not found");
                }
            }

            var createdAvailableTimes = new List<AvailableTimeDto>();

            foreach (var timeSlot in request.Dto.TimeSlots)
            {
                var isAvailable = await _availableTimeRepository.IsTimeSlotAvailableAsync(
                    request.Dto.BranchId,
                    timeSlot.Time,
                    request.Dto.AppointmentTypeId);

                if (isAvailable)
                {
                    var availableTime = new AvailableTime
                    {
                        Time = timeSlot.Time,
                        BranchId = request.Dto.BranchId,
                        AppointmentTypeId = request.Dto.AppointmentTypeId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    var created = await _availableTimeRepository.AddAsync(availableTime);
                    var dto = _mapper.Map<AvailableTimeDto>(created);
                    createdAvailableTimes.Add(dto);
                }
            }

            return Result.Success(createdAvailableTimes);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<AvailableTimeDto>>($"Error bulk creating available times: {ex.Message}");
        }
    }
}