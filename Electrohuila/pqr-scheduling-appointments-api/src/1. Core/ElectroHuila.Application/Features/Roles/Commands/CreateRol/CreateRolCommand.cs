using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Commands.CreateRol;

public record CreateRolCommand(CreateRolDto RolDto) : IRequest<Result<RolDto>>;