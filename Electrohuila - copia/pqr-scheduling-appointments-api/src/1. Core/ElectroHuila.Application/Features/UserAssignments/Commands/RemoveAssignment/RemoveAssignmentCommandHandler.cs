using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.UserAssignments.Commands.RemoveAssignment;

/// <summary>
/// Handler para el comando de eliminar una asignación
/// </summary>
public class RemoveAssignmentCommandHandler : IRequestHandler<RemoveAssignmentCommand, Result<bool>>
{
    private readonly IUserAssignmentRepository _assignmentRepository;

    public RemoveAssignmentCommandHandler(IUserAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<Result<bool>> Handle(RemoveAssignmentCommand request, CancellationToken cancellationToken)
    {
        // Verificar que la asignación existe y está activa
        var isActive = await _assignmentRepository.IsActiveAsync(request.AssignmentId);
        if (!isActive)
        {
            return Result.Failure<bool>("Asignación no encontrada o ya está inactiva");
        }

        // Eliminar la asignación
        await _assignmentRepository.DeleteAsync(request.AssignmentId);

        return Result.Success(true);
    }
}
