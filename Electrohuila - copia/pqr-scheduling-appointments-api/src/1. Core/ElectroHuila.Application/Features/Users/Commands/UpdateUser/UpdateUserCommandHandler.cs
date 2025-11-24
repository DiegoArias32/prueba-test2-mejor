using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Users;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
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
            existingUser.AllowedTabs = request.UserDto.AllowedTabs;
            existingUser.UpdatedAt = DateTime.UtcNow;

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