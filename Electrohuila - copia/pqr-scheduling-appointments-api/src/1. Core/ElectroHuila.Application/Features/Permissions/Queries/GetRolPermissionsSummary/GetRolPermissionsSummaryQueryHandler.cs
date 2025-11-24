using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Auth;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Queries.GetRolPermissionsSummary;

public class GetRolPermissionsSummaryQueryHandler : IRequestHandler<GetRolPermissionsSummaryQuery, Result<RolPermissionSummaryDto>>
{
    private readonly IRolRepository _rolRepository;
    private readonly IMapper _mapper;

    public GetRolPermissionsSummaryQueryHandler(IRolRepository rolRepository, IMapper mapper)
    {
        _rolRepository = rolRepository;
        _mapper = mapper;
    }

    public async Task<Result<RolPermissionSummaryDto>> Handle(GetRolPermissionsSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _rolRepository.GetByIdAsync(request.RolId);
            if (role == null)
            {
                return Result.Failure<RolPermissionSummaryDto>($"Role with ID {request.RolId} not found");
            }

            var activePermissions = role.RolFormPermis.Where(rfp => rfp.IsActive).ToList();
            var formPermissions = activePermissions.Select(rfp => new FormPermissionDto
            {
                FormId = rfp.FormId,
                FormName = rfp.Form?.Name ?? string.Empty,
                FormCode = rfp.Form?.Code ?? string.Empty,
                CanRead = rfp.Permission?.CanRead ?? false,
                CanCreate = rfp.Permission?.CanCreate ?? false,
                CanUpdate = rfp.Permission?.CanUpdate ?? false,
                CanDelete = rfp.Permission?.CanDelete ?? false
            }).ToList();

            var summary = new RolPermissionSummaryDto
            {
                RoleId = role.Id,
                RoleName = role.Name,
                RoleCode = role.Code,
                FormPermissions = formPermissions,
                TotalPermissions = role.RolFormPermis.Count,
                ActivePermissions = activePermissions.Count
            };

            return Result.Success(summary);
        }
        catch (Exception ex)
        {
            return Result.Failure<RolPermissionSummaryDto>($"Error retrieving role permissions summary: {ex.Message}");
        }
    }
}