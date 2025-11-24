using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Users;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery() : IRequest<Result<IEnumerable<UserDto>>>;