using ElectroHuila.Application.Common.Interfaces.Services.Common;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Auth.Queries.GetUserInfo;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, Result<object>>
{
    private readonly ICurrentUserService _currentUserService;

    public GetUserInfoQueryHandler(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<Result<object>> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserService.UserId;
            var username = _currentUserService.Username;
            var email = _currentUserService.Email;
            var roles = _currentUserService.Roles;

            if (!userId.HasValue)
            {
                return Result.Failure<object>("Usuario no autenticado");
            }

            var userInfo = new
            {
                Id = userId?.ToString(),
                Username = username,
                Email = email,
                Roles = roles
            };

            return await Task.FromResult(Result.Success<object>(userInfo));
        }
        catch (Exception ex)
        {
            return Result.Failure<object>($"Error obtaining user info: {ex.Message}");
        }
    }
}
