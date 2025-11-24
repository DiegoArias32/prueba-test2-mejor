using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Queries.GetRolFormPermissionsAssignments;

public class GetRolFormPermissionsAssignmentsQueryHandler : IRequestHandler<GetRolFormPermissionsAssignmentsQuery, Result<object>>
{
    private readonly IRolFormPermissionRepository _rolFormPermissionRepository;

    public GetRolFormPermissionsAssignmentsQueryHandler(IRolFormPermissionRepository rolFormPermissionRepository)
    {
        _rolFormPermissionRepository = rolFormPermissionRepository;
    }

    public async Task<Result<object>> Handle(GetRolFormPermissionsAssignmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var assignments = await _rolFormPermissionRepository.GetRolFormPermissionsAssignmentsAsync(request.RolId, request.FormId);
            return Result.Success<object>(assignments);
        }
        catch (Exception ex)
        {
            return Result.Failure<object>($"Error al obtener las asignaciones de permisos: {ex.Message}");
        }
    }
}
