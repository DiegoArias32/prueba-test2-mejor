using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Commands.DeleteLogicalBranch;

/// <summary>
/// Manejador del comando para eliminación lógica de sucursales
/// </summary>
public class DeleteLogicalBranchCommandHandler : IRequestHandler<DeleteLogicalBranchCommand, Result>
{
    private readonly IBranchRepository _branchRepository;

    public DeleteLogicalBranchCommandHandler(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }

    public async Task<Result> Handle(DeleteLogicalBranchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var branch = await _branchRepository.GetByIdAsync(request.Id);
            if (branch == null)
            {
                return Result.Failure($"Sucursal con ID {request.Id} no encontrada");
            }

            if (!branch.IsActive)
            {
                return Result.Failure($"La sucursal con ID {request.Id} ya está inactiva");
            }

            // Eliminación lógica
            branch.IsActive = false;
            branch.UpdatedAt = DateTime.UtcNow;
            await _branchRepository.UpdateAsync(branch);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error al eliminar lógicamente la sucursal: {ex.Message}");
        }
    }
}
