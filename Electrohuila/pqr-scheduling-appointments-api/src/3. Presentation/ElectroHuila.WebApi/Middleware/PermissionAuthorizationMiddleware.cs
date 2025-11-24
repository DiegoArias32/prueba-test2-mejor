using ElectroHuila.Application.Contracts.Repositories;
using System.Security.Claims;

namespace ElectroHuila.WebApi.Middleware;

/// <summary>
/// Middleware para autorización basada en permisos de usuario.
/// Valida que los usuarios autenticados estén activos y agrega información del usuario al contexto HTTP.
/// </summary>
public class PermissionAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PermissionAuthorizationMiddleware> _logger;

    /// <summary>
    /// Constructor del middleware de autorización por permisos.
    /// </summary>
    /// <param name="next">Siguiente middleware en el pipeline.</param>
    /// <param name="logger">Logger para registrar intentos de acceso no autorizados.</param>
    public PermissionAuthorizationMiddleware(RequestDelegate next, ILogger<PermissionAuthorizationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invoca el middleware y valida el estado del usuario autenticado.
    /// </summary>
    /// <param name="context">Contexto HTTP de la solicitud actual.</param>
    /// <param name="userRepository">Repositorio de usuarios inyectado desde el scope del request.</param>
    /// <remarks>
    /// Flujo de autorización:
    /// 1. Verifica si el usuario está autenticado
    /// 2. Obtiene el ID del usuario desde los claims del JWT
    /// 3. Consulta el usuario en la base de datos
    /// 4. Valida que el usuario esté activo (IsActive = true)
    /// 5. Si está inactivo, retorna 403 Forbidden
    /// 6. Si está activo, agrega el usuario al contexto HTTP para uso posterior
    /// </remarks>
    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
    {
        var user = context.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var userEntity = await userRepository.GetByIdAsync(userId);

                if (userEntity != null && !userEntity.IsActive)
                {
                    _logger.LogWarning("Inactive user {UserId} attempted to access {Path}", userId, context.Request.Path);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { error = "User account is inactive" });
                    return;
                }

                if (userEntity != null)
                {
                    context.Items["User"] = userEntity;
                    context.Items["UserId"] = userId;
                }
            }
        }

        await _next(context);
    }
}