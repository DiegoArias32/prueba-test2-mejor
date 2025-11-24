using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using ElectroHuila.Domain.Entities.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Commands.CreateAppointmentType;

public class CreateAppointmentTypeCommandHandler : IRequestHandler<CreateAppointmentTypeCommand, Result<AppointmentTypeDto>>
{
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IMapper _mapper;

    public CreateAppointmentTypeCommandHandler(IAppointmentTypeRepository appointmentTypeRepository, IMapper mapper)
    {
        _appointmentTypeRepository = appointmentTypeRepository;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentTypeDto>> Handle(CreateAppointmentTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _appointmentTypeRepository.ExistsByNameAsync(request.AppointmentTypeDto.Name);
            if (exists)
            {
                return Result.Failure<AppointmentTypeDto>($"Appointment type with name '{request.AppointmentTypeDto.Name}' already exists");
            }

            var appointmentType = AppointmentType.Create(
                code: request.AppointmentTypeDto.Name.ToUpperInvariant().Replace(" ", "_"),
                name: request.AppointmentTypeDto.Name,
                description: request.AppointmentTypeDto.Description,
                iconName: request.AppointmentTypeDto.Icon,
                estimatedTimeMinutes: request.AppointmentTypeDto.EstimatedTimeMinutes,
                requiresDocumentation: request.AppointmentTypeDto.RequiresDocumentation
            );

            var createdAppointmentType = await _appointmentTypeRepository.AddAsync(appointmentType);
            var appointmentTypeDto = _mapper.Map<AppointmentTypeDto>(createdAppointmentType);

            return Result.Success(appointmentTypeDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentTypeDto>($"Error creating appointment type: {ex.Message}");
        }
    }
}