namespace ElectroHuila.Application.Contracts.Notifications;

/// <summary>
/// Servicio de notificaciones en tiempo real usando SignalR.
/// Permite enviar notificaciones a usuarios, roles y broadcast.
/// </summary>
public interface ISignalRNotificationService
{
    /// <summary>
    /// Envía una notificación a un usuario específico
    /// </summary>
    /// <param name="userId">ID del usuario destinatario</param>
    /// <param name="notification">Objeto de notificación a enviar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    Task SendNotificationToUserAsync(string userId, object notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía una notificación a todos los usuarios con un rol específico
    /// </summary>
    /// <param name="roleName">Nombre del rol destinatario</param>
    /// <param name="notification">Objeto de notificación a enviar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    Task SendNotificationToRoleAsync(string roleName, object notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía una notificación a todos los usuarios conectados (broadcast)
    /// </summary>
    /// <param name="notification">Objeto de notificación a enviar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    Task BroadcastNotificationAsync(object notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía una notificación a un grupo específico
    /// </summary>
    /// <param name="groupName">Nombre del grupo destinatario</param>
    /// <param name="notification">Objeto de notificación a enviar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    Task SendNotificationToGroupAsync(string groupName, object notification, CancellationToken cancellationToken = default);
}
