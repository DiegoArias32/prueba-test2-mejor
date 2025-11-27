using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Users;
using ElectroHuila.Domain.Entities.Security;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUserRepository userRepository, IRolRepository rolRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _rolRepository = rolRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingUser = await _userRepository.GetByIdAsync(request.Id);
            if (existingUser == null)
            {
                return Result.Failure<UserDto>($"User with ID {request.Id} not found");
            }

            if (!string.IsNullOrEmpty(request.UserDto.Username) && request.UserDto.Username != existingUser.Username)
            {
                var usernameExists = await _userRepository.ExistsByUsernameAsync(request.UserDto.Username);
                if (usernameExists)
                {
                    return Result.Failure<UserDto>($"Username '{request.UserDto.Username}' already exists");
                }
            }

            if (!string.IsNullOrEmpty(request.UserDto.Email) && request.UserDto.Email != existingUser.Email)
            {
                var emailExists = await _userRepository.ExistsByEmailAsync(request.UserDto.Email);
                if (emailExists)
                {
                    return Result.Failure<UserDto>($"Email '{request.UserDto.Email}' already exists");
                }
            }

            existingUser.Username = request.UserDto.Username;
            existingUser.Email = request.UserDto.Email;
            existingUser.FullName = request.UserDto.FullName;
            existingUser.IdentificationType = request.UserDto.IdentificationType;
            existingUser.IdentificationNumber = request.UserDto.IdentificationNumber;
            existingUser.Phone = request.UserDto.Phone;
            existingUser.Address = request.UserDto.Address;
            existingUser.AllowedTabs = request.UserDto.AllowedTabs;
            existingUser.UpdatedAt = DateTime.UtcNow;

            // Update IsActive if provided
            if (request.UserDto.IsActive.HasValue)
            {
                existingUser.IsActive = request.UserDto.IsActive.Value;
            }

            // Update roles if provided
            if (request.UserDto.RoleIds != null)
            {
                // Remove existing role assignments
                existingUser.RolUsers.Clear();

                // Add new role assignments
                foreach (var roleId in request.UserDto.RoleIds)
                {
                    var role = await _rolRepository.GetByIdAsync(roleId);
                    if (role != null)
                    {
                        var rolUser = new RolUser
                        {
                            UserId = existingUser.Id,
                            RolId = roleId,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };
                        existingUser.RolUsers.Add(rolUser);
                    }
                }
            }

            await _userRepository.UpdateAsync(existingUser);
            var userDto = _mapper.Map<UserDto>(existingUser);

            return Result.Success(userDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<UserDto>($"Error updating user: {ex.Message}");
        }
    }
}