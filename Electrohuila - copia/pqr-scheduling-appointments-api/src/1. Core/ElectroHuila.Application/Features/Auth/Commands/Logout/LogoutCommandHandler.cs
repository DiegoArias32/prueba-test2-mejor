using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // Implement token invalidation logic here
        // This typically involves adding the token to a blacklist or revoking it in the database
        await Task.CompletedTask;
        return Result.Success(true);
    }
}
