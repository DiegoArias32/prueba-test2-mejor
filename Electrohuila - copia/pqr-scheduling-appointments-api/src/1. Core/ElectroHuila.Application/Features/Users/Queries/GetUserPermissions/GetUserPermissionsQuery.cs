using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Permissions;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Queries.GetUserPermissions;

public record GetUserPermissionsQuery(int UserId) : IRequest<Result<IEnumerable<PermissionDto>>>;