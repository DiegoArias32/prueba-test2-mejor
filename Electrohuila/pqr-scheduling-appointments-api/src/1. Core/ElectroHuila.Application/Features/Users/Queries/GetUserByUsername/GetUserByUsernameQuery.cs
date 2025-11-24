using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Users;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Queries.GetUserByUsername;

public record GetUserByUsernameQuery(string Username) : IRequest<Result<UserDetailsDto>>;