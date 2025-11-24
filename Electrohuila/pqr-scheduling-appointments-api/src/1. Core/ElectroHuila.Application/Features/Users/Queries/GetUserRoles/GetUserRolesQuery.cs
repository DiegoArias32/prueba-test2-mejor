using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Queries.GetUserRoles;

public record GetUserRolesQuery(int UserId) : IRequest<Result<IEnumerable<RolDto>>>;