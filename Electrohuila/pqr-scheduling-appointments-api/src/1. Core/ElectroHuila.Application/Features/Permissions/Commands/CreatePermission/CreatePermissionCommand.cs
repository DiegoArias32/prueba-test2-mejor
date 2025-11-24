using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Permissions;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Commands.CreatePermission;

public record CreatePermissionCommand(CreatePermissionDto Dto) : IRequest<Result<PermissionDto>>;