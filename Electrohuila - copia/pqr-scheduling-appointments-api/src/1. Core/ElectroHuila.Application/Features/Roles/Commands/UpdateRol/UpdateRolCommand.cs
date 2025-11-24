using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Commands.UpdateRol;

public record UpdateRolCommand(int Id, UpdateRolDto RolDto) : IRequest<Result<RolDto>>;