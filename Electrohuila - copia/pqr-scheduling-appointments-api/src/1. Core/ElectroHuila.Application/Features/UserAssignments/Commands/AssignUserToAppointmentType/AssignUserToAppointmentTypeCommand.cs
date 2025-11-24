using ElectroHuila.Application.DTOs.Assignments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.UserAssignments.Commands.AssignUserToAppointmentType;

/// <summary>
/// Comando para asignar un usuario a un tipo de cita
/// </summary>
public record AssignUserToAppointmentTypeCommand(CreateAssignmentDto Dto) : IRequest<Result<UserAssignmentDto>>;
