using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Queries.GetAllIncludingInactive;

/// <summary>
/// Query para obtener todos los roles incluyendo los inactivos.
/// </summary>
public record GetAllRolesIncludingInactiveQuery : IRequest<Result<IEnumerable<RolDto>>>;
