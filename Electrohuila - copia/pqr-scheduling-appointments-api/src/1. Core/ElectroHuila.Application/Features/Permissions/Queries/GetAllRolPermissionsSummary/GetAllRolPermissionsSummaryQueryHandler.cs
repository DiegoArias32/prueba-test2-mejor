using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Queries.GetAllRolPermissionsSummary;

public class GetAllRolPermissionsSummaryQueryHandler : IRequestHandler<GetAllRolPermissionsSummaryQuery, Result<object>>
{
    private readonly IRolFormPermissionRepository _rolFormPermissionRepository;

    public GetAllRolPermissionsSummaryQueryHandler(IRolFormPermissionRepository rolFormPermissionRepository)
    {
        _rolFormPermissionRepository = rolFormPermissionRepository;
    }

    public async Task<Result<object>> Handle(GetAllRolPermissionsSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var summary = await _rolFormPermissionRepository.GetAllRolPermissionsSummaryAsync();
            return Result.Success<object>(summary);
        }
        catch (Exception ex)
        {
            return Result.Failure<object>($"Error al obtener el resumen de permisos: {ex.Message}");
        }
    }
}
