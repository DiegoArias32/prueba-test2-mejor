using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Users;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Queries.GetAllIncludingInactive;

/// <summary>
/// Handler para obtener todos los usuarios incluyendo los inactivos.
/// </summary>
public class GetAllUsersIncludingInactiveQueryHandler
    : IRequestHandler<GetAllUsersIncludingInactiveQuery, Result<IEnumerable<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersIncludingInactiveQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<UserDto>>> Handle(
        GetAllUsersIncludingInactiveQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var users = await _userRepository.GetAllIncludingInactiveAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            return Result.Success(userDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<UserDto>>($"Error retrieving users including inactive: {ex.Message}");
        }
    }
}
