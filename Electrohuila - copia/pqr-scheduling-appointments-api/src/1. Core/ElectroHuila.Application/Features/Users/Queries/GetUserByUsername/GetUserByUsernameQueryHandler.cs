using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Users;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Queries.GetUserByUsername;

public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, Result<UserDetailsDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByUsernameQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserDetailsDto>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return Result.Failure<UserDetailsDto>("Username cannot be empty");
            }

            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                return Result.Failure<UserDetailsDto>($"User with username '{request.Username}' not found");
            }

            var userDetailsDto = _mapper.Map<UserDetailsDto>(user);
            return Result.Success(userDetailsDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<UserDetailsDto>($"Error retrieving user: {ex.Message}");
        }
    }
}