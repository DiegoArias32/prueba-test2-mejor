using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Permissions;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Commands.AssignPermissionToRol;

public record AssignPermissionToRolCommand(AssignPermissionToRolDto Dto) : IRequest<Result<bool>>;