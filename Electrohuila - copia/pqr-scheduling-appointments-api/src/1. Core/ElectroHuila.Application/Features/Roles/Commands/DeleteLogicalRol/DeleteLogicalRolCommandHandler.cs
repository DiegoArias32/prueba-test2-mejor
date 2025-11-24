using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Commands.DeleteLogicalRol;

public class DeleteLogicalRolCommandHandler : IRequestHandler<DeleteLogicalRolCommand, Result<object>>
{
    private readonly IRolRepository _rolRepository;

    public DeleteLogicalRolCommandHandler(IRolRepository rolRepository)
    {
        _rolRepository = rolRepository;
    }

    public async Task<Result<object>> Handle(DeleteLogicalRolCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var rol = await _rolRepository.GetByIdAsync(request.RolId);

            if (rol == null)
            {
                return Result.Failure<object>("Rol no encontrado");
            }

            await _rolRepository.DeleteLogicalAsync(request.RolId);

            return Result.Success<object>(new { message = "Rol eliminado lógicamente exitosamente" });
        }
        catch (Exception ex)
        {
            return Result.Failure<object>($"Error al eliminar el rol: {ex.Message}");
        }
    }
}
