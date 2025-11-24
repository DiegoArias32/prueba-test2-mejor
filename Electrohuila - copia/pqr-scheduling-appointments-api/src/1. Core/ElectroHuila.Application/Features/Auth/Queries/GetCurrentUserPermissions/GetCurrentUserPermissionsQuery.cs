using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Auth.Queries.GetCurrentUserPermissions;

public record GetCurrentUserPermissionsQuery() : IRequest<Result<object>>;
