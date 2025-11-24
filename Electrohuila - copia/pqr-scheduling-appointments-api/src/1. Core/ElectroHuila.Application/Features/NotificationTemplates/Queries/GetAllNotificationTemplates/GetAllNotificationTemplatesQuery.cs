using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;

namespace ElectroHuila.Application.Features.NotificationTemplates.Queries.GetAllNotificationTemplates;

/// <summary>
/// Query para obtener todas las plantillas de notificación
/// </summary>
public record GetAllNotificationTemplatesQuery : IRequest<Result<IEnumerable<NotificationTemplateDto>>>;
