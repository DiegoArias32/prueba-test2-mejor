using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Permissions;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, Result<IEnumerable<PermissionDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;

    public GetUserPermissionsQueryHandler(
        IUserRepository userRepository,
        IRolRepository rolRepository,
        IPermissionRepository permissionRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _rolRepository = rolRepository;
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<PermissionDto>>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result.Failure<IEnumerable<PermissionDto>>($"User with ID {request.UserId} not found");
            }

            var roles = await _rolRepository.GetByUserIdAsync(request.UserId);
            var allPermissions = new List<PermissionDto>();

            foreach (var role in roles)
            {
                var permissions = await _permissionRepository.GetByRolIdAsync(role.Id);
                var permissionDtos = _mapper.Map<IEnumerable<PermissionDto>>(permissions);
                allPermissions.AddRange(permissionDtos);
            }

            // Remove duplicates
            var uniquePermissions = allPermissions.GroupBy(p => p.Id).Select(g => g.First());

            return Result.Success(uniquePermissions);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<PermissionDto>>($"Error retrieving user permissions: {ex.Message}");
        }
    }
}