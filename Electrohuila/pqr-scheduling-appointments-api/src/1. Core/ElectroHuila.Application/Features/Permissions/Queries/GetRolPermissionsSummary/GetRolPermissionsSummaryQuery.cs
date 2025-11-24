using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Queries.GetRolPermissionsSummary;

public record GetRolPermissionsSummaryQuery(int RolId) : IRequest<Result<RolPermissionSummaryDto>>;