namespace ElectroHuila.Application.Common.Interfaces.Services.Common;

/// <summary>
/// Servicio para acceder a la información del usuario autenticado actualmente
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Obtiene el ID del usuario autenticado
    /// </summary>
    int? UserId { get; }

    /// <summary>
    /// Obtiene el nombre de usuario del usuario autenticado
    /// </summary>
    string? Username { get; }

    /// <summary>
    /// Obtiene el email del usuario autenticado
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Indica si el usuario está autenticado
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Obtiene los roles del usuario autenticado
    /// </summary>
    IEnumerable<string> Roles { get; }

    /// <summary>
    /// Obtiene los permisos del usuario autenticado
    /// </summary>
    IEnumerable<string> Permissions { get; }

    /// <summary>
    /// Verifica si el usuario tiene un permiso específico
    /// </summary>
    /// <param name="permission">Nombre del permiso a verificar</param>
    /// <returns>True si el usuario tiene el permiso</returns>
    bool HasPermission(string permission);

    /// <summary>
    /// Verifica si el usuario tiene un rol específico
    /// </summary>
    /// <param name="role">Nombre del rol a verificar</param>
    /// <returns>True si el usuario tiene el rol</returns>
    bool HasRole(string role);

    /// <summary>
    /// Verifica si el usuario tiene al menos uno de los permisos especificados
    /// </summary>
    /// <param name="permissions">Permisos a verificar</param>
    /// <returns>True si el usuario tiene al menos uno de los permisos</returns>
    bool HasAnyPermission(params string[] permissions);

    /// <summary>
    /// Verifica si el usuario tiene todos los permisos especificados
    /// </summary>
    /// <param name="permissions">Permisos a verificar</param>
    /// <returns>True si el usuario tiene todos los permisos</returns>
    bool HasAllPermissions(params string[] permissions);
}