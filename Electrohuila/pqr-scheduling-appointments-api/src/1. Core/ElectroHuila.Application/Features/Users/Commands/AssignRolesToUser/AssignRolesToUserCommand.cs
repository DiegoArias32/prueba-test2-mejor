using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Commands.AssignRolesToUser;

public record AssignRolesToUserCommand(int UserId, List<int> RoleIds) : IRequest<Result<bool>>;