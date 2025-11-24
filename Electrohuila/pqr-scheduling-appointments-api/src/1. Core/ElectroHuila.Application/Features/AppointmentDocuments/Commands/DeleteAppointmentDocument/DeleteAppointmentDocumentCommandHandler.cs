using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentDocuments.Commands.DeleteAppointmentDocument;

/// <summary>
/// Manejador del comando para eliminar un documento adjunto
/// </summary>
public class DeleteAppointmentDocumentCommandHandler : IRequestHandler<DeleteAppointmentDocumentCommand, Result<bool>>
{
    private readonly IAppointmentDocumentRepository _repository;

    public DeleteAppointmentDocumentCommandHandler(IAppointmentDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle(DeleteAppointmentDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _repository.GetByIdAsync(request.Id);
        if (document == null)
        {
            return Result.Failure<bool>($"Documento con ID {request.Id} no encontrado");
        }

        // Soft delete
        document.IsActive = false;
        document.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(document);

        return Result.Success(true);
    }
}
