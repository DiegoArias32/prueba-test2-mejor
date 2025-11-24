using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Users;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<Result<UserDetailsDto>>;