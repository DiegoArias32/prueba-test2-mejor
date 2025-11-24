using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Commands.UpdateUserTabs;

public class UpdateUserTabsCommandHandler : IRequestHandler<UpdateUserTabsCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserTabsCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<bool>> Handle(UpdateUserTabsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.Dto.UserId);
            if (user == null)
            {
                return Result.Failure<bool>($"User with ID {request.Dto.UserId} not found");
            }

            user.AllowedTabs = request.Dto.AllowedTabs;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error updating user tabs: {ex.Message}");
        }
    }
}