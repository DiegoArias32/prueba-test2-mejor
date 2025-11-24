using Microsoft.AspNetCore.Authorization;

namespace ElectroHuila.Infrastructure.Attributes;

/// <summary>
/// Atributo de autorización personalizado que requiere un permiso específico para acceder a un recurso.
/// Hereda de <see cref="AuthorizeAttribute"/> para proporcionar autorización basada en permisos.
/// </summary>
/// <remarks>
/// Este atributo se utiliza para decorar controladores o métodos de acción que requieren permisos específicos.
/// Internamente genera una política de autorización con el formato "Permission:{permission}".
/// </remarks>
/// <example>
/// <code>
/// [RequirePermission("users.create")]
/// public IActionResult CreateUser() { ...}
/// </code>
/// </example>
public class RequirePermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Inicializa una nueva instancia del atributo <see cref="RequirePermissionAttribute"/>.
    /// </summary>
    /// <param name="permission">
    /// El nombre del permiso requerido para acceder al recurso.
    /// Se recomienda usar el formato "modulo.accion" (ej: "appointments.read").
    /// </param>
    public RequirePermissionAttribute(string permission)
    {
        Policy = $"Permission:{permission}";
    }
}