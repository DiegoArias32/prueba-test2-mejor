using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Users;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(CreateUserDto UserDto) : IRequest<Result<UserDto>>;