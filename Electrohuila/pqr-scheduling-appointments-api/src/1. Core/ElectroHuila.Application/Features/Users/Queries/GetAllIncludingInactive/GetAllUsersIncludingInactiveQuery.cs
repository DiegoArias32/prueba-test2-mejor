using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Users;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Queries.GetAllIncludingInactive;

/// <summary>
/// Query para obtener todos los usuarios incluyendo los inactivos.
/// </summary>
public record GetAllUsersIncludingInactiveQuery : IRequest<Result<IEnumerable<UserDto>>>;
