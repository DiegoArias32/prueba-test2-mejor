using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Services;
using MediatR;
using System.Security.Claims;

namespace ElectroHuila.Application.Features.Auth.Queries.ValidateToken;

public class ValidateTokenQueryHandler : IRequestHandler<ValidateTokenQuery, Result<object>>
{
    private readonly ITokenService _tokenService;

    public ValidateTokenQueryHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<Result<object>> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var principal = _tokenService.ValidateToken(request.Token);

            if (principal == null)
            {
                return Result.Failure<object>("Token inválido o expirado");
            }

            var response = new
            {
                valid = true,
                user = principal.FindFirst(ClaimTypes.Name)?.Value,
                userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            };

            return Result.Success<object>(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<object>($"Error validating token: {ex.Message}");
        }
    }
}
