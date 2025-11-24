using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.AvailableTimes;
using ElectroHuila.Domain.Entities.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Commands.CreateAvailableTime;

public class CreateAvailableTimeCommandHandler : IRequestHandler<CreateAvailableTimeCommand, Result<AvailableTimeDto>>
{
    private readonly IAvailableTimeRepository _availableTimeRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IMapper _mapper;

    public CreateAvailableTimeCommandHandler(
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

    public async Task<Result<AvailableTimeDto>> Handle(CreateAvailableTimeCommand request, CancellationToken cancellationToken)
    {
        try
        {
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

            var isAvailable = await _availableTimeRepository.IsTimeSlotAvailableAsync(
                request.AvailableTimeDto.BranchId,
                request.AvailableTimeDto.Time,
                request.AvailableTimeDto.AppointmentTypeId);

            if (!isAvailable)
            {
                return Result.Failure<AvailableTimeDto>("Time slot already exists for this branch and appointment type");
            }

            var availableTime = new AvailableTime
            {
                Time = request.AvailableTimeDto.Time,
                BranchId = request.AvailableTimeDto.BranchId,
                AppointmentTypeId = request.AvailableTimeDto.AppointmentTypeId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdAvailableTime = await _availableTimeRepository.AddAsync(availableTime);
            var availableTimeDto = _mapper.Map<AvailableTimeDto>(createdAvailableTime);

            return Result.Success(availableTimeDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AvailableTimeDto>($"Error creating available time: {ex.Message}");
        }
    }
}