using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Queries.GetAllRolPermissionsSummary;

public record GetAllRolPermissionsSummaryQuery() : IRequest<Result<object>>;
