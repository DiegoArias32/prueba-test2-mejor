using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;

namespace ElectroHuila.Application.Features.NotificationTemplates.Queries.GetNotificationTemplateByCode;

/// <summary>
/// Query para obtener una plantilla de notificación por código
/// </summary>
public record GetNotificationTemplateByCodeQuery(string TemplateCode) : IRequest<Result<NotificationTemplateDto>>;
