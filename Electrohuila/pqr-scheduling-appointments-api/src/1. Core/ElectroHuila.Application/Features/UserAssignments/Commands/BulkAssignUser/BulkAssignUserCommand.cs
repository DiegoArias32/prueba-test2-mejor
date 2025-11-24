using ElectroHuila.Application.DTOs.Assignments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.UserAssignments.Commands.BulkAssignUser;

/// <summary>
/// Comando para asignar múltiples tipos de cita a un usuario
/// </summary>
public record BulkAssignUserCommand(BulkAssignmentDto Dto) : IRequest<Result<List<UserAssignmentDto>>>;
