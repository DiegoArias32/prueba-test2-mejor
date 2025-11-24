using ElectroHuila.Application.DTOs.Assignments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.UserAssignments.Queries.GetUserAssignments;

/// <summary>
/// Query para obtener todas las asignaciones de un usuario
/// </summary>
public record GetUserAssignmentsQuery(int UserId) : IRequest<Result<List<UserAssignmentDto>>>;
