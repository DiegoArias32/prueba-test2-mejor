using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Queries.GetAllRoles;

public record GetAllRolesQuery() : IRequest<Result<IEnumerable<RolDto>>>;