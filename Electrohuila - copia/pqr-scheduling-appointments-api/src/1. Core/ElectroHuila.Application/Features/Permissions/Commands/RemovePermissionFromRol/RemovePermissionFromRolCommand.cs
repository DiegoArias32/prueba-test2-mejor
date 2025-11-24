using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Permissions;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Commands.RemovePermissionFromRol;

public record RemovePermissionFromRolCommand(RemovePermissionFromRolDto Dto) : IRequest<Result<bool>>;