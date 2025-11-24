using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.UserAssignments.Commands.RemoveAssignment;

/// <summary>
/// Comando para eliminar una asignación de usuario a tipo de cita
/// </summary>
public record RemoveAssignmentCommand(int AssignmentId) : IRequest<Result<bool>>;
