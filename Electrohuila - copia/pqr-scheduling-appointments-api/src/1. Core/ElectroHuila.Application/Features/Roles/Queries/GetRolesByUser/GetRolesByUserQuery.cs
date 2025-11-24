using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Queries.GetRolesByUser;

public record GetRolesByUserQuery(int UserId) : IRequest<Result<IEnumerable<RolDto>>>;