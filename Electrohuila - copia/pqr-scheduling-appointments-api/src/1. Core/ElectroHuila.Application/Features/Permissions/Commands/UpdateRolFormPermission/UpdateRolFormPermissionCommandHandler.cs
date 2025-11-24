using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Commands.UpdateRolFormPermission;

public class UpdateRolFormPermissionCommandHandler : IRequestHandler<UpdateRolFormPermissionCommand, Result<object>>
{
    private readonly IRolFormPermissionRepository _rolFormPermissionRepository;

    public UpdateRolFormPermissionCommandHandler(IRolFormPermissionRepository rolFormPermissionRepository)
    {
        _rolFormPermissionRepository = rolFormPermissionRepository;
    }

    public async Task<Result<object>> Handle(UpdateRolFormPermissionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Dto;

            if (dto.RolId <= 0 || dto.FormId <= 0)
            {
                return Result.Failure<object>("RolId y FormId son requeridos");
            }

            await _rolFormPermissionRepository.UpdateRolFormPermissionAsync(
                dto.RolId,
                dto.FormId,
                dto.CanInsert,
                dto.CanUpdate,
                dto.CanDelete,
                dto.CanView
            );

            return Result.Success<object>(new { message = "Permiso actualizado correctamente" });
        }
        catch (Exception ex)
        {
            return Result.Failure<object>($"Error al actualizar el permiso: {ex.Message}");
        }
    }
}
