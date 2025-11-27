using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Commands.UpdateAppointmentType;

public class UpdateAppointmentTypeCommandHandler : IRequestHandler<UpdateAppointmentTypeCommand, Result<AppointmentTypeDto>>
{
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IMapper _mapper;

    public UpdateAppointmentTypeCommandHandler(IAppointmentTypeRepository appointmentTypeRepository, IMapper mapper)
    {
        _appointmentTypeRepository = appointmentTypeRepository;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentTypeDto>> Handle(UpdateAppointmentTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingAppointmentType = await _appointmentTypeRepository.GetByIdAsync(request.Id);
            if (existingAppointmentType == null)
            {
                return Result.Failure<AppointmentTypeDto>($"Appointment type with ID {request.Id} not found");
            }

            // Check if name is being changed and validate uniqueness
            if (!string.IsNullOrEmpty(request.AppointmentTypeDto.Name) && request.AppointmentTypeDto.Name != existingAppointmentType.Name)
            {
                var nameExists = await _appointmentTypeRepository.ExistsByNameAsync(request.AppointmentTypeDto.Name);
                if (nameExists)
                {
                    return Result.Failure<AppointmentTypeDto>($"Appointment type with name '{request.AppointmentTypeDto.Name}' already exists");
                }
            }

            // Only update fields that are provided (not null)
            if (!string.IsNullOrEmpty(request.AppointmentTypeDto.Name) || !string.IsNullOrEmpty(request.AppointmentTypeDto.Description))
            {
                var name = !string.IsNullOrEmpty(request.AppointmentTypeDto.Name)
                    ? request.AppointmentTypeDto.Name
                    : existingAppointmentType.Name;
                var description = request.AppointmentTypeDto.Description ?? existingAppointmentType.Description;
                existingAppointmentType.UpdateDetails(name, description);
            }

            if (!string.IsNullOrEmpty(request.AppointmentTypeDto.Icon))
            {
                existingAppointmentType.UpdateDesign(request.AppointmentTypeDto.Icon, null, null);
            }

            if (request.AppointmentTypeDto.EstimatedTimeMinutes.HasValue && request.AppointmentTypeDto.EstimatedTimeMinutes.Value > 0)
            {
                var requiresDoc = request.AppointmentTypeDto.RequiresDocumentation ?? existingAppointmentType.RequiresDocumentation;
                existingAppointmentType.UpdateConfiguration(
                    request.AppointmentTypeDto.EstimatedTimeMinutes.Value,
                    requiresDoc);
            }

            // Update IsActive if provided
            if (request.AppointmentTypeDto.IsActive.HasValue)
            {
                existingAppointmentType.IsActive = request.AppointmentTypeDto.IsActive.Value;
                existingAppointmentType.UpdatedAt = DateTime.UtcNow;
            }

            await _appointmentTypeRepository.UpdateAsync(existingAppointmentType);
            var appointmentTypeDto = _mapper.Map<AppointmentTypeDto>(existingAppointmentType);

            return Result.Success(appointmentTypeDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentTypeDto>($"Error updating appointment type: {ex.Message}");
        }
    }
}