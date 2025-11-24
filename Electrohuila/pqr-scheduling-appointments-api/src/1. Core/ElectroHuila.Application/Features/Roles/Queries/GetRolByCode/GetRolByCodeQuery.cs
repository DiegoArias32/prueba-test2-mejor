using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Queries.GetRolByCode;

public record GetRolByCodeQuery(string Code) : IRequest<Result<RolDto>>;