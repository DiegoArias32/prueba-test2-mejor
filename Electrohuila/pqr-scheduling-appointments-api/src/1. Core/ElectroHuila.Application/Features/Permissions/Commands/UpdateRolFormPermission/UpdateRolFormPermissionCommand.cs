using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Permissions;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Commands.UpdateRolFormPermission;

public record UpdateRolFormPermissionCommand(UpdateRolFormPermissionDto Dto) : IRequest<Result<object>>;
