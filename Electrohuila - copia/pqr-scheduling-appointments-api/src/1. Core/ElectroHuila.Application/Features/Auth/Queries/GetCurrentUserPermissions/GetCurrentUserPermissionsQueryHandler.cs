using ElectroHuila.Application.Common.Interfaces.Services.Common;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Auth.Queries.GetCurrentUserPermissions;

public class GetCurrentUserPermissionsQueryHandler : IRequestHandler<GetCurrentUserPermissionsQuery, Result<object>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;

    public GetCurrentUserPermissionsQueryHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
    }

    public async Task<Result<object>> Handle(GetCurrentUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserService.UserId;

            if (!userId.HasValue)
            {
                return Result.Failure<object>("Usuario no válido");
            }

            var permissions = await _userRepository.GetUserPermissionsAsync(userId.Value);

            return Result.Success<object>(permissions);
        }
        catch (Exception ex)
        {
            return Result.Failure<object>($"Error obtaining user permissions: {ex.Message}");
        }
    }
}
