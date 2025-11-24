using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Security;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Commands.AssignRolesToUser;

public class AssignRolesToUserCommandHandler : IRequestHandler<AssignRolesToUserCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRolRepository _rolRepository;

    public AssignRolesToUserCommandHandler(IUserRepository userRepository, IRolRepository rolRepository)
    {
        _userRepository = userRepository;
        _rolRepository = rolRepository;
    }

    public async Task<Result<bool>> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result.Failure<bool>($"User with ID {request.UserId} not found");
            }

            // Validate all roles exist
            foreach (var roleId in request.RoleIds)
            {
                var roleExists = await _rolRepository.ExistsAsync(roleId);
                if (!roleExists)
                {
                    return Result.Failure<bool>($"Role with ID {roleId} not found");
                }
            }

            // Clear existing roles
            user.RolUsers.Clear();

            // Assign new roles
            foreach (var roleId in request.RoleIds)
            {
                user.RolUsers.Add(new RolUser
                {
                    UserId = request.UserId,
                    RolId = roleId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error assigning roles to user: {ex.Message}");
        }
    }
}