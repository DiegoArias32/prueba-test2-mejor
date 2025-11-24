using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Permissions;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Queries.GetAllPermissions;

public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, Result<IEnumerable<PermissionDto>>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;

    public GetAllPermissionsQueryHandler(IPermissionRepository permissionRepository, IMapper mapper)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<PermissionDto>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var permissions = await _permissionRepository.GetAllAsync();
            var permissionDtos = _mapper.Map<IEnumerable<PermissionDto>>(permissions);

            return Result.Success(permissionDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<PermissionDto>>($"Error retrieving permissions: {ex.Message}");
        }
    }
}