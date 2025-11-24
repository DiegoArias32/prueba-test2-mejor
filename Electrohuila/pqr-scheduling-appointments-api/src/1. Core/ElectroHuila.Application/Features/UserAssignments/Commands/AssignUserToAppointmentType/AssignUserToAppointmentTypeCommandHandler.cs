using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Assignments;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Domain.Entities.Assignments;
using MediatR;

namespace ElectroHuila.Application.Features.UserAssignments.Commands.AssignUserToAppointmentType;

/// <summary>
/// Handler para el comando de asignar un usuario a un tipo de cita
/// </summary>
public class AssignUserToAppointmentTypeCommandHandler : IRequestHandler<AssignUserToAppointmentTypeCommand, Result<UserAssignmentDto>>
{
    private readonly IUserAssignmentRepository _assignmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IMapper _mapper;

    public AssignUserToAppointmentTypeCommandHandler(
        IUserAssignmentRepository assignmentRepository,
        IUserRepository userRepository,
        IAppointmentTypeRepository appointmentTypeRepository,
        IMapper mapper)
    {
        _assignmentRepository = assignmentRepository;
        _userRepository = userRepository;
        _appointmentTypeRepository = appointmentTypeRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserAssignmentDto>> Handle(AssignUserToAppointmentTypeCommand request, CancellationToken cancellationToken)
    {
        // Validar que el usuario existe
        var userExists = await _userRepository.ExistsAsync(request.Dto.UserId);
        if (!userExists)
        {
            return Result.Failure<UserAssignmentDto>($"Usuario con ID {request.Dto.UserId} no encontrado");
        }

        // Validar que el tipo de cita existe
        var appointmentTypeExists = await _appointmentTypeRepository.ExistsAsync(request.Dto.AppointmentTypeId);
        if (!appointmentTypeExists)
        {
            return Result.Failure<UserAssignmentDto>($"Tipo de cita con ID {request.Dto.AppointmentTypeId} no encontrado");
        }

        // Verificar si ya existe la asignación
        var exists = await _assignmentRepository.ExistsAsync(request.Dto.UserId, request.Dto.AppointmentTypeId);
        if (exists)
        {
            return Result.Failure<UserAssignmentDto>("Esta asignación ya existe");
        }

        // Crear la asignación
        var assignment = new UserAppointmentTypeAssignment
        {
            UserId = request.Dto.UserId,
            AppointmentTypeId = request.Dto.AppointmentTypeId,
            IsActive = true
        };

        var createdAssignment = await _assignmentRepository.AddAsync(assignment);
        var assignmentDto = _mapper.Map<UserAssignmentDto>(createdAssignment);

        return Result.Success(assignmentDto);
    }
}
