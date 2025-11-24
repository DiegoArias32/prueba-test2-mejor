using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Queries.GetRolFormPermissionsAssignments;

public record GetRolFormPermissionsAssignmentsQuery(int? RolId, int? FormId) : IRequest<Result<object>>;
