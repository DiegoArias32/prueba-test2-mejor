using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.NotificationTemplates.Commands.DeleteNotificationTemplate;

/// <summary>
/// Manejador del comando para eliminar una plantilla de notificación
/// </summary>
public class DeleteNotificationTemplateCommandHandler : IRequestHandler<DeleteNotificationTemplateCommand, Result<bool>>
{
    private readonly INotificationTemplateRepository _repository;

    public DeleteNotificationTemplateCommandHandler(INotificationTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle(DeleteNotificationTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(request.Id);
        if (template == null)
        {
            return Result.Failure<bool>($"Plantilla con ID {request.Id} no encontrada");
        }

        // Soft delete
        template.IsActive = false;
        template.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(template);

        return Result.Success(true);
    }
}
