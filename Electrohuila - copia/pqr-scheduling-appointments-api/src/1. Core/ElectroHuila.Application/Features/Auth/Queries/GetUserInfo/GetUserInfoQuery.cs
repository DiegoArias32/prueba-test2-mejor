using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Auth.Queries.GetUserInfo;

public record GetUserInfoQuery() : IRequest<Result<object>>;
