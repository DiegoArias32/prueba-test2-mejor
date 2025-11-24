using ElectroHuila.Application.Common.Interfaces;
using ElectroHuila.Application.Common.Interfaces.Services.Common;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ElectroHuila.Infrastructure.Identity;

/// <summary>
/// Servicio para acceder a la información del usuario autenticado en el contexto HTTP actual.
/// Implementa <see cref="ICurrentUserService"/> para proporcionar acceso a claims, roles y permisos.
/// </summary>
/// <remarks>
/// Este servicio utiliza <see cref="IHttpContextAccessor"/> para extraer información del usuario
/// desde los claims del contexto HTTP actual. Proporciona métodos de conveniencia para verificar
/// autenticación, roles y permisos de manera type-safe.
/// </remarks>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="CurrentUserService"/>.
    /// </summary>
    /// <param name="httpContextAccessor">Accessor para obtener el contexto HTTP actual.</param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Obtiene el identificador único del usuario autenticado actual.
    /// </summary>
    /// <value>
    /// El ID del usuario como entero si está autenticado y el claim es válido; de lo contrario, null.
    /// </value>
    /// <remarks>
    /// Extrae el valor del claim <see cref="ClaimTypes.NameIdentifier"/> y lo convierte a entero.
    /// </remarks>
    public int? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    /// <summary>
    /// Obtiene el nombre de usuario del usuario autenticado actual.
    /// </summary>
    /// <value>
    /// El nombre de usuario como string si está disponible; de lo contrario, null.
    /// </value>
    /// <remarks>
    /// Extrae el valor del claim <see cref="ClaimTypes.Name"/>.
    /// </remarks>
    public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

    /// <summary>
    /// Obtiene la dirección de correo electrónico del usuario autenticado actual.
    /// </summary>
    /// <value>
    /// El email como string si está disponible; de lo contrario, null.
    /// </value>
    /// <remarks>
    /// Extrae el valor del claim <see cref="ClaimTypes.Email"/>.
    /// </remarks>
    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    /// <summary>
    /// Obtiene el primer nombre del usuario autenticado actual.
    /// </summary>
    /// <value>
    /// El primer nombre como string si está disponible; de lo contrario, null.
    /// </value>
    /// <remarks>
    /// Extrae el valor del claim <see cref="ClaimTypes.GivenName"/>.
    /// </remarks>
    public string? FirstName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value;

    /// <summary>
    /// Obtiene el apellido del usuario autenticado actual.
    /// </summary>
    /// <value>
    /// El apellido como string si está disponible; de lo contrario, null.
    /// </value>
    /// <remarks>
    /// Extrae el valor del claim <see cref="ClaimTypes.Surname"/>.
    /// </remarks>
    public string? LastName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Surname)?.Value;

    /// <summary>
    /// Obtiene todos los roles asignados al usuario autenticado actual.
    /// </summary>
    /// <value>
    /// Una colección de strings con los nombres de los roles; colección vacía si no hay roles.
    /// </value>
    /// <remarks>
    /// Extrae todos los claims de tipo <see cref="ClaimTypes.Role"/>.
    /// </remarks>
    public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value) ?? Enumerable.Empty<string>();

    /// <summary>
    /// Obtiene todos los permisos asignados al usuario autenticado actual.
    /// </summary>
    /// <value>
    /// Una colección de strings con los nombres de los permisos; colección vacía si no hay permisos.
    /// </value>
    /// <remarks>
    /// Extrae todos los claims de tipo "Permission".
    /// </remarks>
    public IEnumerable<string> Permissions => _httpContextAccessor.HttpContext?.User?.FindAll("Permission")?.Select(c => c.Value) ?? Enumerable.Empty<string>();

    /// <summary>
    /// Indica si el usuario actual está autenticado.
    /// </summary>
    /// <value>
    /// true si el usuario está autenticado; de lo contrario, false.
    /// </value>
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// Verifica si el usuario actual tiene un permiso específico.
    /// </summary>
    /// <param name="permission">El nombre del permiso a verificar.</param>
    /// <returns>true si el usuario tiene el permiso; de lo contrario, false.</returns>
    public bool HasPermission(string permission)
    {
        return Permissions.Contains(permission);
    }

    /// <summary>
    /// Verifica si el usuario actual tiene un rol específico.
    /// </summary>
    /// <param name="role">El nombre del rol a verificar.</param>
    /// <returns>true si el usuario tiene el rol; de lo contrario, false.</returns>
    public bool HasRole(string role)
    {
        return Roles.Contains(role);
    }

    /// <summary>
    /// Verifica si el usuario actual tiene al menos uno de los permisos especificados.
    /// </summary>
    /// <param name="permissions">Lista de permisos a verificar.</param>
    /// <returns>true si el usuario tiene al menos uno de los permisos; de lo contrario, false.</returns>
    public bool HasAnyPermission(params string[] permissions)
    {
        return permissions.Any(p => Permissions.Contains(p));
    }

    /// <summary>
    /// Verifica si el usuario actual tiene todos los permisos especificados.
    /// </summary>
    /// <param name="permissions">Lista de permisos que debe tener el usuario.</param>
    /// <returns>true si el usuario tiene todos los permisos; de lo contrario, false.</returns>
    public bool HasAllPermissions(params string[] permissions)
    {
        return permissions.All(p => Permissions.Contains(p));
    }

    /// <summary>
    /// Verifica si el usuario actual pertenece a un rol específico usando el método nativo de Identity.
    /// </summary>
    /// <param name="role">El nombre del rol a verificar.</param>
    /// <returns>true si el usuario pertenece al rol; de lo contrario, false.</returns>
    /// <remarks>
    /// Este método utiliza el método <see cref="ClaimsPrincipal.IsInRole(string)"/> de ASP.NET Core Identity.
    /// </remarks>
    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }

    /// <summary>
    /// Obtiene el valor de un claim específico del usuario autenticado actual.
    /// </summary>
    /// <param name="claimType">El tipo de claim a buscar.</param>
    /// <returns>El valor del claim como string si existe; de lo contrario, null.</returns>
    /// <remarks>
    /// Método genérico para extraer cualquier tipo de claim no cubierto por las propiedades específicas.
    /// </remarks>
    public string? GetClaimValue(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
    }
}