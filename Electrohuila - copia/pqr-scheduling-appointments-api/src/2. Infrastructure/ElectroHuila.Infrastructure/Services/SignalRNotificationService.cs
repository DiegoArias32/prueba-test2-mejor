using ElectroHuila.Application.Contracts.Notifications;
using ElectroHuila.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de notificaciones en tiempo real usando SignalR.
/// </summary>
public class SignalRNotificationService : ISignalRNotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<SignalRNotificationService> _logger;

    /// <summary>
    /// Constructor del servicio SignalR
    /// </summary>
    /// <param name="hubContext">Contexto del hub de notificaciones</param>
    /// <param name="logger">Logger para registrar eventos</param>
    public SignalRNotificationService(
        IHubContext<NotificationHub> hubContext,
        ILogger<SignalRNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Envía una notificación a un usuario específico
    /// </summary>
    /// <param name="userId">ID del usuario destinatario</param>
    /// <param name="notification">Objeto de notificación a enviar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    public async Task SendNotificationToUserAsync(string userId, object notification, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Attempted to send notification to empty userId");
                return;
            }

            var groupName = $"user_{userId}";
            _logger.LogInformation("Sending SignalR notification to user {UserId}", userId);

            await _hubContext.Clients.Group(groupName)
                .SendAsync("ReceiveNotification", notification, cancellationToken);

            _logger.LogDebug("SignalR notification sent successfully to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SignalR notification to user {UserId}", userId);
            // No lanzamos la excepción para evitar que falle el flujo principal si SignalR falla
        }
    }

    /// <summary>
    /// Envía una notificación a todos los usuarios con un rol específico
    /// </summary>
    /// <param name="roleName">Nombre del rol destinatario</param>
    /// <param name="notification">Objeto de notificación a enviar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    public async Task SendNotificationToRoleAsync(string roleName, object notification, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                _logger.LogWarning("Attempted to send notification to empty roleName");
                return;
            }

            var groupName = $"role_{roleName}";
            _logger.LogInformation("Sending SignalR notification to role {RoleName}", roleName);

            await _hubContext.Clients.Group(groupName)
                .SendAsync("ReceiveNotification", notification, cancellationToken);

            _logger.LogDebug("SignalR notification sent successfully to role {RoleName}", roleName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SignalR notification to role {RoleName}", roleName);
            // No lanzamos la excepción para evitar que falle el flujo principal si SignalR falla
        }
    }

    /// <summary>
    /// Envía una notificación a todos los usuarios conectados (broadcast)
    /// </summary>
    /// <param name="notification">Objeto de notificación a enviar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    public async Task BroadcastNotificationAsync(object notification, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Broadcasting SignalR notification to all connected clients");

            await _hubContext.Clients.All
                .SendAsync("ReceiveNotification", notification, cancellationToken);

            _logger.LogDebug("SignalR notification broadcast successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error broadcasting SignalR notification");
            // No lanzamos la excepción para evitar que falle el flujo principal si SignalR falla
        }
    }

    /// <summary>
    /// Envía una notificación a un grupo específico
    /// </summary>
    /// <param name="groupName">Nombre del grupo destinatario</param>
    /// <param name="notification">Objeto de notificación a enviar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    public async Task SendNotificationToGroupAsync(string groupName, object notification, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                _logger.LogWarning("Attempted to send notification to empty groupName");
                return;
            }

            _logger.LogInformation("Sending SignalR notification to group {GroupName}", groupName);

            await _hubContext.Clients.Group(groupName)
                .SendAsync("ReceiveNotification", notification, cancellationToken);

            _logger.LogDebug("SignalR notification sent successfully to group {GroupName}", groupName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SignalR notification to group {GroupName}", groupName);
            // No lanzamos la excepción para evitar que falle el flujo principal si SignalR falla
        }
    }
}
