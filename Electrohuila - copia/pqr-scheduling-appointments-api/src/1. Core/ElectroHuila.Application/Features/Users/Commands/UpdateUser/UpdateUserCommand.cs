using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Users;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(int Id, UpdateUserDto UserDto) : IRequest<Result<UserDto>>;