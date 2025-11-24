using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Auth.Queries.ValidateToken;

public record ValidateTokenQuery(string Token) : IRequest<Result<object>>;
