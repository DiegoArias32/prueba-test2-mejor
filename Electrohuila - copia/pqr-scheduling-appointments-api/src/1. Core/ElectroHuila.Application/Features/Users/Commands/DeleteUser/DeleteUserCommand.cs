using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(int Id) : IRequest<Result<bool>>;