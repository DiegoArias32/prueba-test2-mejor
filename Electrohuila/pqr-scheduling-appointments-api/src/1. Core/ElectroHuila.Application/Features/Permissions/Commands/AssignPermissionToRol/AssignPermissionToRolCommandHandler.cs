using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Security;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Commands.AssignPermissionToRol;

public class AssignPermissionToRolCommandHandler : IRequestHandler<AssignPermissionToRolCommand, Result<bool>>
{
    private readonly IRolRepository _rolRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IFormRepository _formRepository;

    public AssignPermissionToRolCommandHandler(
        IRolRepository rolRepository,
        IPermissionRepository permissionRepository,
        IFormRepository formRepository)
    {
        _rolRepository = rolRepository;
        _permissionRepository = permissionRepository;
        _formRepository = formRepository;
    }

    public async Task<Result<bool>> Handle(AssignPermissionToRolCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _rolRepository.GetByIdAsync(request.Dto.RolId);
            if (role == null)
            {
                return Result.Failure<bool>($"Role with ID {request.Dto.RolId} not found");
            }

            var formExists = await _formRepository.ExistsAsync(request.Dto.FormId);
            if (!formExists)
            {
                return Result.Failure<bool>($"Form with ID {request.Dto.FormId} not found");
            }

            // Create permission
            var permission = new Permission
            {
                CanRead = request.Dto.CanRead,
                CanCreate = request.Dto.CanCreate,
                CanUpdate = request.Dto.CanUpdate,
                CanDelete = request.Dto.CanDelete,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            permission = await _permissionRepository.AddAsync(permission);

            // Create RolFormPermi relationship
            var rolFormPermi = new RolFormPermi
            {
                RolId = request.Dto.RolId,
                FormId = request.Dto.FormId,
                PermissionId = permission.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            role.RolFormPermis.Add(rolFormPermi);
            role.UpdatedAt = DateTime.UtcNow;
            await _rolRepository.UpdateAsync(role);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error assigning permission to role: {ex.Message}");
        }
    }
}