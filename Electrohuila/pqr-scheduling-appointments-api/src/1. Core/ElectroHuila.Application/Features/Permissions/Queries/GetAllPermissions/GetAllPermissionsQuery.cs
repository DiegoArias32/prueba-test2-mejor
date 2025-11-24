using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Permissions;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Queries.GetAllPermissions;

public record GetAllPermissionsQuery() : IRequest<Result<IEnumerable<PermissionDto>>>;