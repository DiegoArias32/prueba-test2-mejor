using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Auth;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Queries.GetRolFormPermissions;

public record GetRolFormPermissionsQuery(int RolId, int FormId) : IRequest<Result<FormPermissionDto>>;