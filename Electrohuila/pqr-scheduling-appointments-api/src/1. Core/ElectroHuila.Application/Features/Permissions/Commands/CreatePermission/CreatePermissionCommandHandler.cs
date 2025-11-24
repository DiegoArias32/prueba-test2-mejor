using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Permissions;
using ElectroHuila.Domain.Entities.Security;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Commands.CreatePermission;

public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, Result<PermissionDto>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;

    public CreatePermissionCommandHandler(IPermissionRepository permissionRepository, IMapper mapper)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }

    public async Task<Result<PermissionDto>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var permission = new Permission
            {
                CanRead = request.Dto.CanRead,
                CanCreate = request.Dto.CanCreate,
                CanUpdate = request.Dto.CanUpdate,
                CanDelete = request.Dto.CanDelete,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdPermission = await _permissionRepository.AddAsync(permission);
            var permissionDto = _mapper.Map<PermissionDto>(createdPermission);

            return Result.Success(permissionDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<PermissionDto>($"Error creating permission: {ex.Message}");
        }
    }
}