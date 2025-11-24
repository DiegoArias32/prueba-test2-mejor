using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Auth;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Queries.GetRolFormPermissions;

public class GetRolFormPermissionsQueryHandler : IRequestHandler<GetRolFormPermissionsQuery, Result<FormPermissionDto>>
{
    private readonly IRolRepository _rolRepository;
    private readonly IFormRepository _formRepository;

    public GetRolFormPermissionsQueryHandler(IRolRepository rolRepository, IFormRepository formRepository)
    {
        _rolRepository = rolRepository;
        _formRepository = formRepository;
    }

    public async Task<Result<FormPermissionDto>> Handle(GetRolFormPermissionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _rolRepository.GetByIdAsync(request.RolId);
            if (role == null)
            {
                return Result.Failure<FormPermissionDto>($"Role with ID {request.RolId} not found");
            }

            var form = await _formRepository.GetByIdAsync(request.FormId);
            if (form == null)
            {
                return Result.Failure<FormPermissionDto>($"Form with ID {request.FormId} not found");
            }

            var rolFormPermi = role.RolFormPermis
                .FirstOrDefault(rfp => rfp.FormId == request.FormId && rfp.IsActive);

            if (rolFormPermi == null)
            {
                return Result.Failure<FormPermissionDto>($"No permission found for Role {request.RolId} and Form {request.FormId}");
            }

            var formPermission = new FormPermissionDto
            {
                FormId = form.Id,
                FormName = form.Name,
                FormCode = form.Code,
                CanRead = rolFormPermi.Permission?.CanRead ?? false,
                CanCreate = rolFormPermi.Permission?.CanCreate ?? false,
                CanUpdate = rolFormPermi.Permission?.CanUpdate ?? false,
                CanDelete = rolFormPermi.Permission?.CanDelete ?? false
            };

            return Result.Success(formPermission);
        }
        catch (Exception ex)
        {
            return Result.Failure<FormPermissionDto>($"Error retrieving role form permissions: {ex.Message}");
        }
    }
}