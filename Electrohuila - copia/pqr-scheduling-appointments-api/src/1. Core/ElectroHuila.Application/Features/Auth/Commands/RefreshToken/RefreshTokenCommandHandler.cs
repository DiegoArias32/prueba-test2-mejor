using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Auth;
using MediatR;

namespace ElectroHuila.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponseDto>>
{
    public async Task<Result<LoginResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Implement token refresh logic here
        await Task.CompletedTask;
        return Result.Failure<LoginResponseDto>("Token refresh not implemented");
    }
}
