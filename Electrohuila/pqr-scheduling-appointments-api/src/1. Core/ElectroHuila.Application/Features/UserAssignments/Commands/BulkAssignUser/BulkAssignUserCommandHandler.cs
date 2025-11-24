using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Assignments;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Domain.Entities.Assignments;
using MediatR;

namespace ElectroHuila.Application.Features.UserAssignments.Commands.BulkAssignUser;

/// <summary>
/// Handler para el comando de asignación en masa de tipos de cita a un usuario
/// </summary>
public class BulkAssignUserCommandHandler : IRequestHandler<BulkAssignUserCommand, Result<List<UserAssignmentDto>>>
{
    private readonly IUserAssignmentRepository _assignmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IMapper _mapper;

    public BulkAssignUserCommandHandler(
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

    public async Task<Result<List<UserAssignmentDto>>> Handle(BulkAssignUserCommand request, CancellationToken cancellationToken)
    {
        // Validar que el usuario existe
        var userExists = await _userRepository.ExistsAsync(request.Dto.UserId);
        if (!userExists)
        {
            return Result.Failure<List<UserAssignmentDto>>($"Usuario con ID {request.Dto.UserId} no encontrado");
        }

        var createdAssignments = new List<UserAssignmentDto>();
        var errors = new List<string>();

        foreach (var appointmentTypeId in request.Dto.AppointmentTypeIds)
        {
            // Validar que el tipo de cita existe
            var appointmentTypeExists = await _appointmentTypeRepository.ExistsAsync(appointmentTypeId);
            if (!appointmentTypeExists)
            {
                errors.Add($"Tipo de cita con ID {appointmentTypeId} no encontrado");
                continue;
            }

            // Verificar si ya existe la asignación
            var exists = await _assignmentRepository.ExistsAsync(request.Dto.UserId, appointmentTypeId);
            if (exists)
            {
                errors.Add($"Asignación para tipo de cita {appointmentTypeId} ya existe");
                continue;
            }

            // Crear la asignación
            var assignment = new UserAppointmentTypeAssignment
            {
                UserId = request.Dto.UserId,
                AppointmentTypeId = appointmentTypeId,
                IsActive = true
            };

            var createdAssignment = await _assignmentRepository.AddAsync(assignment);
            var assignmentDto = _mapper.Map<UserAssignmentDto>(createdAssignment);
            createdAssignments.Add(assignmentDto);
        }

        // Si no se creó ninguna asignación, retornar error
        if (createdAssignments.Count == 0)
        {
            return Result.Failure<List<UserAssignmentDto>>(
                $"No se pudo crear ninguna asignación. Errores: {string.Join(", ", errors)}"
            );
        }

        // Retornar éxito con las asignaciones creadas
        return Result.Success(createdAssignments);
    }
}
