using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.NotificationTemplates.Commands.DeleteNotificationTemplate;

/// <summary>
/// Comando para eliminar (desactivar) una plantilla de notificación
/// </summary>
public record DeleteNotificationTemplateCommand(int Id) : IRequest<Result<bool>>;
