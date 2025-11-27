using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.AvailableTimes;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Commands.UpdateAvailableTime;

public class UpdateAvailableTimeCommandHandler : IRequestHandler<UpdateAvailableTimeCommand, Result<AvailableTimeDto>>
{
    private readonly IAvailableTimeRepository _availableTimeRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IMapper _mapper;

    public UpdateAvailableTimeCommandHandler(
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

    public async Task<Result<AvailableTimeDto>> Handle(UpdateAvailableTimeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingAvailableTime = await _availableTimeRepository.GetByIdAsync(request.Id);
            if (existingAvailableTime == null)
            {
                return Result.Failure<AvailableTimeDto>($"Available time with ID {request.Id} not found");
            }

            var branchExists = await _branchRepository.ExistsAsync(request.AvailableTimeDto.BranchId);
            if (!branchExists)
            {
                return Result.Failure<AvailableTimeDto>($"Branch with ID {request.AvailableTimeDto.BranchId} not found");
            }

            if (request.AvailableTimeDto.AppointmentTypeId.HasValue)
            {
                var appointmentTypeExists = await _appointmentTypeRepository.ExistsAsync(request.AvailableTimeDto.AppointmentTypeId.Value);
                if (!appointmentTypeExists)
                {
                    return Result.Failure<AvailableTimeDto>($"Appointment type with ID {request.AvailableTimeDto.AppointmentTypeId.Value} not found");
                }
            }

            existingAvailableTime.Time = request.AvailableTimeDto.Time;
            existingAvailableTime.BranchId = request.AvailableTimeDto.BranchId;
            existingAvailableTime.AppointmentTypeId = request.AvailableTimeDto.AppointmentTypeId;
            existingAvailableTime.UpdatedAt = DateTime.UtcNow;

            // Update IsActive if provided
            if (request.AvailableTimeDto.IsActive.HasValue)
            {
                existingAvailableTime.IsActive = request.AvailableTimeDto.IsActive.Value;
            }

            await _availableTimeRepository.UpdateAsync(existingAvailableTime);
            var availableTimeDto = _mapper.Map<AvailableTimeDto>(existingAvailableTime);

            return Result.Success(availableTimeDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AvailableTimeDto>($"Error updating available time: {ex.Message}");
        }
    }
}