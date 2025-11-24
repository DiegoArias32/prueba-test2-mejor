using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Notifications;
using ElectroHuila.Application.Features.Notifications.Commands.CreateNotification;
using ElectroHuila.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using ElectroHuila.Application.Features.Notifications.Queries.GetMyNotifications;
using ElectroHuila.Application.Features.Notifications.Queries.GetUnreadCount;
using ElectroHuila.Application.Features.Notifications.Queries.GetUserNotifications;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controller for managing notifications in the system.
/// Provides endpoints to create, retrieve, and mark notifications as read.
/// All endpoints require JWT authentication.
/// </summary>
[Authorize]
public class NotificationsController : ApiController
{
    private readonly ILogger<NotificationsController> _logger;

    /// <summary>
    /// Initializes a new instance of the NotificationsController.
    /// </summary>
    /// <param name="logger">Logger for diagnostic information.</param>
    public NotificationsController(ILogger<NotificationsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Retrieves notifications for the authenticated user based on their assigned appointment types.
    /// Includes:
    /// - IN_APP notifications directed to the user
    /// - EMAIL/WHATSAPP notifications for clients with appointments in the user's assigned types
    /// Returns notifications ordered by creation date (newest first).
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Number of items per page (default: 20, max: 100).</param>
    /// <returns>Paginated list of notifications relevant to the user.</returns>
    /// <response code="200">Returns the paginated list of notifications.</response>
    /// <response code="400">If validation fails or parameters are invalid.</response>
    /// <response code="401">If user is not authenticated.</response>
    [HttpGet("my-notifications")]
    [ProducesResponseType(typeof(PagedList<NotificationListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyNotifications(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation(
            "GetMyNotifications called - Page: {Page}, PageSize: {PageSize}",
            pageNumber,
            pageSize
        );

        var query = new GetMyNotificationsQuery(pageNumber, pageSize);
        var result = await Mediator.Send(query);

        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves notifications for a specific user with pagination.
    /// Returns notifications ordered by creation date (newest first).
    /// </summary>
    /// <param name="userId">ID of the user whose notifications to retrieve.</param>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Number of items per page (default: 20, max: 100).</param>
    /// <returns>Paginated list of notifications for the user.</returns>
    /// <response code="200">Returns the list of notifications.</response>
    /// <response code="400">If validation fails or parameters are invalid.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user tries to access another user's notifications.</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<NotificationListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserNotifications(
        int userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation(
            "GetUserNotifications called - UserId: {UserId}, Page: {Page}, PageSize: {PageSize}",
            userId,
            pageNumber,
            pageSize
        );

        // Get the authenticated user ID from JWT
        var authenticatedUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(authenticatedUserIdClaim) || !int.TryParse(authenticatedUserIdClaim, out int authenticatedUserId))
        {
            _logger.LogWarning("Invalid or missing userId in JWT token");
            return Unauthorized(new { message = "Usuario no autenticado o ID inválido" });
        }

        // Check if user is trying to access their own notifications or is admin
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var isAdmin = userRoles.Contains("Admin") || userRoles.Contains("ADMIN");

        if (authenticatedUserId != userId && !isAdmin)
        {
            _logger.LogWarning(
                "User {AuthenticatedUserId} attempted to access notifications of user {TargetUserId}",
                authenticatedUserId,
                userId
            );
            return Forbid();
        }

        var query = new GetUserNotificationsQuery(userId, pageNumber, pageSize);
        var result = await Mediator.Send(query);

        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves the count of unread notifications for the authenticated user.
    /// Only counts IN_APP notifications that have not been read.
    /// </summary>
    /// <returns>Number of unread notifications.</returns>
    /// <response code="200">Returns the unread count.</response>
    /// <response code="401">If user is not authenticated.</response>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUnreadCount()
    {
        // Get the authenticated user ID from JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            _logger.LogWarning("Invalid or missing userId in JWT token");
            return Unauthorized(new { message = "Usuario no autenticado o ID inválido" });
        }

        _logger.LogInformation("GetUnreadCount called for user {UserId}", userId);

        var query = new GetUnreadCountQuery(userId);
        var result = await Mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(new { unreadCount = result.Data });
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves the count of unread notifications for a specific user.
    /// Admin endpoint - allows checking any user's unread count.
    /// </summary>
    /// <param name="userId">ID of the user whose unread count to retrieve.</param>
    /// <returns>Number of unread notifications.</returns>
    /// <response code="200">Returns the unread count.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user is not an admin.</response>
    [HttpGet("user/{userId}/unread-count")]
    [Authorize(Roles = "Admin,ADMIN,Super Administrator")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserUnreadCount(int userId)
    {
        _logger.LogInformation("GetUserUnreadCount called for user {UserId}", userId);

        var query = new GetUnreadCountQuery(userId);
        var result = await Mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(new { userId, unreadCount = result.Data });
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Marks a notification as read by the authenticated user.
    /// Only the owner of the notification can mark it as read.
    /// </summary>
    /// <param name="id">ID of the notification to mark as read.</param>
    /// <returns>Success confirmation.</returns>
    /// <response code="200">Notification marked as read successfully.</response>
    /// <response code="400">If the notification doesn't exist or validation fails.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user doesn't own the notification.</response>
    [HttpPatch("{id}/mark-read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        // Get the authenticated user ID from JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            _logger.LogWarning("Invalid or missing userId in JWT token");
            return Unauthorized(new { message = "Usuario no autenticado o ID inválido" });
        }

        _logger.LogInformation(
            "MarkAsRead called - NotificationId: {NotificationId}, UserId: {UserId}",
            id,
            userId
        );

        var command = new MarkNotificationAsReadCommand(id, userId);
        var result = await Mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Notificación marcada como leída" });
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Creates a new notification in the system.
    /// Admin-only endpoint for manually creating notifications.
    /// </summary>
    /// <param name="dto">Notification data to create.</param>
    /// <returns>Created notification with its assigned ID.</returns>
    /// <response code="201">Notification created successfully.</response>
    /// <response code="400">If validation fails or user doesn't exist.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user is not an admin.</response>
    [HttpPost]
    [Authorize(Roles = "Admin,ADMIN,Super Administrator")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateNotificationDto dto)
    {
        _logger.LogInformation(
            "Create notification called - Type: {Type}, UserId: {UserId}",
            dto.Type,
            dto.UserId
        );

        var command = new CreateNotificationCommand(dto);
        var result = await Mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetUserNotifications),
                new { userId = result.Data!.UserId },
                result.Data
            );
        }

        return HandleResult(result);
    }
}
