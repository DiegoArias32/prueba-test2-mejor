using ElectroHuila.Application.DTOs.Assignments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.UserAssignments.Queries.GetAllAssignments;

/// <summary>
/// Query para obtener todas las asignaciones (admin)
/// </summary>
public record GetAllAssignmentsQuery : IRequest<Result<List<UserAssignmentDto>>>;
