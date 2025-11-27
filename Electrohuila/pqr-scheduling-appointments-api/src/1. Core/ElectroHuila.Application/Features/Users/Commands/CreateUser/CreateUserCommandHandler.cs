using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Users;
using ElectroHuila.Domain.Entities.Security;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IUserRepository userRepository, IRolRepository rolRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _rolRepository = rolRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var usernameExists = await _userRepository.ExistsByUsernameAsync(request.UserDto.Username);
            if (usernameExists)
            {
                return Result.Failure<UserDto>($"Username '{request.UserDto.Username}' already exists");
            }

            var emailExists = await _userRepository.ExistsByEmailAsync(request.UserDto.Email);
            if (emailExists)
            {
                return Result.Failure<UserDto>($"Email '{request.UserDto.Email}' already exists");
            }

            var user = _mapper.Map<User>(request.UserDto);
            // Hash password before saving (implement password hashing service)
            // user.Password = _passwordHasher.HashPassword(request.UserDto.Password);

            var createdUser = await _userRepository.AddAsync(user);

            // Assign roles if provided
            if (request.UserDto.RoleIds != null && request.UserDto.RoleIds.Any())
            {
                foreach (var roleId in request.UserDto.RoleIds)
                {
                    var role = await _rolRepository.GetByIdAsync(roleId);
                    if (role != null)
                    {
                        var rolUser = new RolUser
                        {
                            UserId = createdUser.Id,
                            RolId = roleId,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };
                        createdUser.RolUsers.Add(rolUser);
                    }
                }
                await _userRepository.UpdateAsync(createdUser);
            }

            var userDto = _mapper.Map<UserDto>(createdUser);

            return Result.Success(userDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<UserDto>($"Error creating user: {ex.Message}");
        }
    }
}