using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Queries.GetRolesByUser;

public class GetRolesByUserQueryHandler : IRequestHandler<GetRolesByUserQuery, Result<IEnumerable<RolDto>>>
{
    private readonly IRolRepository _rolRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetRolesByUserQueryHandler(IRolRepository rolRepository, IUserRepository userRepository, IMapper mapper)
    {
        _rolRepository = rolRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<RolDto>>> Handle(GetRolesByUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userExists = await _userRepository.ExistsAsync(request.UserId);
            if (!userExists)
            {
                return Result.Failure<IEnumerable<RolDto>>($"User with ID {request.UserId} not found");
            }

            var roles = await _rolRepository.GetByUserIdAsync(request.UserId);
            var roleDtos = _mapper.Map<IEnumerable<RolDto>>(roles);

            return Result.Success(roleDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<RolDto>>($"Error retrieving roles for user: {ex.Message}");
        }
    }
}