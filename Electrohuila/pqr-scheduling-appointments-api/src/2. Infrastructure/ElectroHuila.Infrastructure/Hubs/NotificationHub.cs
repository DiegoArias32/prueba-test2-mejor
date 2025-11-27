using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ElectroHuila.Infrastructure.Hubs;

/// <summary>
/// Hub de SignalR para notificaciones en tiempo real.
/// Permite enviar notificaciones a usuarios específicos, grupos y broadcast.
/// Los métodos críticos están protegidos con [Authorize] individualmente.
/// </summary>
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    /// <summary>
    /// Constructor del NotificationHub
    /// </summary>
    /// <param name="logger">Logger para registrar eventos del hub</param>
    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Envía una notificación a un usuario específico
    /// </summary>
    /// <param name="userId">ID del usuario destinatario</param>
    /// <param name="notification">Objeto de notificación a enviar</param>
    [Authorize]
    public async Task SendNotificationToUser(string userId, object notification)
    {
        try
        {
            _logger.LogInformation("Sending notification to user {UserId}", userId);
            await Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Envía una notificación a un grupo específico
    /// </summary>
    /// <param name="groupName">Nombre del grupo destinatario</param>
    /// <param name="notification">Objeto de notificación a enviar</param>
    [Authorize]
    public async Task SendNotificationToGroup(string groupName, object notification)
    {
        try
        {
            _logger.LogInformation("Sending notification to group {GroupName}", groupName);
            await Clients.Group(groupName).SendAsync("ReceiveNotification", notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to group {GroupName}", groupName);
            throw;
        }
    }

    /// <summary>
    /// Une al usuario actual a su grupo personal basado en su ID
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    [Authorize]
    public async Task JoinUserGroup(string userId)
    {
        try
        {
            var groupName = $"user_{userId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("User {UserId} joined group {GroupName} with connection {ConnectionId}",
                userId, groupName, Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining user {UserId} to their group", userId);
            throw;
        }
    }

    /// <summary>
    /// Remueve al usuario actual de su grupo personal
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    [Authorize]
    public async Task LeaveUserGroup(string userId)
    {
        try
        {
            var groupName = $"user_{userId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("User {UserId} left group {GroupName} with connection {ConnectionId}",
                userId, groupName, Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user {UserId} from their group", userId);
            throw;
        }
    }

    /// <summary>
    /// Une al usuario actual a un grupo basado en su rol
    /// </summary>
    /// <param name="roleName">Nombre del rol</param>
    [Authorize]
    public async Task JoinRoleGroup(string roleName)
    {
        try
        {
            var groupName = $"role_{roleName}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} joined role group {GroupName}",
                Context.ConnectionId, groupName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining connection to role group {RoleName}", roleName);
            throw;
        }
    }

    /// <summary>
    /// Remueve al usuario actual de un grupo de rol
    /// </summary>
    /// <param name="roleName">Nombre del rol</param>
    [Authorize]
    public async Task LeaveRoleGroup(string roleName)
    {
        try
        {
            var groupName = $"role_{roleName}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} left role group {GroupName}",
                Context.ConnectionId, groupName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing connection from role group {RoleName}", roleName);
            throw;
        }
    }

    /// <summary>
    /// Método ping para verificar la conexión
    /// </summary>
    /// <returns>"pong" como respuesta</returns>
    public Task<string> Ping()
    {
        _logger.LogDebug("Ping received from connection {ConnectionId}", Context.ConnectionId);
        return Task.FromResult("pong");
    }

    /// <summary>
    /// Maneja la conexión de un nuevo cliente al hub
    /// Automáticamente une al usuario a su grupo personal y grupos de roles
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoles = Context.User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            _logger.LogInformation("Client connected to NotificationHub. ConnectionId: {ConnectionId}, UserId: {UserId}",
                Context.ConnectionId, userId ?? "Anonymous");

            // Unir automáticamente al usuario a su grupo personal
            if (!string.IsNullOrEmpty(userId))
            {
                var userGroup = $"user_{userId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, userGroup);
                _logger.LogInformation("User {UserId} automatically joined group {GroupName}", userId, userGroup);

                // Unir automáticamente a los grupos de roles del usuario
                if (userRoles != null && userRoles.Any())
                {
                    foreach (var role in userRoles)
                    {
                        var roleGroup = $"role_{role}";
                        await Groups.AddToGroupAsync(Context.ConnectionId, roleGroup);
                        _logger.LogInformation("User {UserId} automatically joined role group {RoleGroup}", userId, roleGroup);
                    }
                }
            }

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnConnectedAsync for connection {ConnectionId}", Context.ConnectionId);
            throw;
        }
    }

    /// <summary>
    /// Maneja la desconexión de un cliente del hub
    /// </summary>
    /// <param name="exception">Excepción que causó la desconexión, si aplica</param>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (exception != null)
            {
                _logger.LogWarning(exception, "Client disconnected with error. ConnectionId: {ConnectionId}, UserId: {UserId}",
                    Context.ConnectionId, userId ?? "Anonymous");
            }
            else
            {
                _logger.LogInformation("Client disconnected normally. ConnectionId: {ConnectionId}, UserId: {UserId}",
                    Context.ConnectionId, userId ?? "Anonymous");
            }

            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnDisconnectedAsync for connection {ConnectionId}", Context.ConnectionId);
            throw;
        }
    }
}
