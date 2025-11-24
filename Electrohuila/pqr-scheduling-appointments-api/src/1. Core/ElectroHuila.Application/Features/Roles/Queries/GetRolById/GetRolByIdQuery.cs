using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Queries.GetRolById;

public record GetRolByIdQuery(int Id) : IRequest<Result<RolDto>>;