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

            if (!string.IsNullOrEmpty(request.AppointmentTypeDto.Name) && request.AppointmentTypeDto.Name != existingAppointmentType.Name)
            {
                var nameExists = await _appointmentTypeRepository.ExistsByNameAsync(request.AppointmentTypeDto.Name);
                if (nameExists)
                {
                    return Result.Failure<AppointmentTypeDto>($"Appointment type with name '{request.AppointmentTypeDto.Name}' already exists");
                }
            }

            existingAppointmentType.UpdateDetails(request.AppointmentTypeDto.Name, request.AppointmentTypeDto.Description);
            existingAppointmentType.UpdateDesign(request.AppointmentTypeDto.Icon, null, null);
            existingAppointmentType.UpdateConfiguration(request.AppointmentTypeDto.EstimatedTimeMinutes, request.AppointmentTypeDto.RequiresDocumentation);

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