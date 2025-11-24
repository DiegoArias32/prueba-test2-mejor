using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Commands.DeleteRol;

public record DeleteRolCommand(int Id) : IRequest<Result<bool>>;