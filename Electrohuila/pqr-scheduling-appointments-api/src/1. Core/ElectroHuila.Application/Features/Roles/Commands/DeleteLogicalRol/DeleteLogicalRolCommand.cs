using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Commands.DeleteLogicalRol;

public record DeleteLogicalRolCommand(int RolId) : IRequest<Result<object>>;
