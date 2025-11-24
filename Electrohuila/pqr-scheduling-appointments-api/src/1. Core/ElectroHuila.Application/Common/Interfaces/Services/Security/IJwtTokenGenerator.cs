using ElectroHuila.Domain.Entities.Security;

namespace ElectroHuila.Application.Common.Interfaces.Services.Security;

/// <summary>
/// Servicio para generar y validar tokens JWT
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Genera un token JWT para un usuario con sus roles y permisos
    /// </summary>
    /// <param name="user">Usuario para el cual generar el token</param>
    /// <param name="roles">Roles del usuario</param>
    /// <param name="permissions">Permisos del usuario</param>
    /// <returns>Token JWT generado</returns>
    string GenerateToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);

    /// <summary>
    /// Genera un refresh token para renovar tokens expirados
    /// </summary>
    /// <returns>Refresh token generado</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Valida si un token JWT es válido
    /// </summary>
    /// <param name="token">Token a validar</param>
    /// <returns>True si el token es válido</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// Obtiene la fecha de expiración de un token
    /// </summary>
    /// <param name="token">Token del cual obtener la expiración</param>
    /// <returns>Fecha y hora de expiración del token</returns>
    DateTime GetTokenExpiration(string token);

    /// <summary>
    /// Extrae el ID de usuario de un token
    /// </summary>
    /// <param name="token">Token del cual extraer el ID</param>
    /// <returns>ID del usuario o null si no se encuentra</returns>
    string? GetUserIdFromToken(string token);

    /// <summary>
    /// Extrae el nombre de usuario de un token
    /// </summary>
    /// <param name="token">Token del cual extraer el nombre de usuario</param>
    /// <returns>Nombre de usuario o null si no se encuentra</returns>
    string? GetUsernameFromToken(string token);

    /// <summary>
    /// Extrae los roles de un token
    /// </summary>
    /// <param name="token">Token del cual extraer los roles</param>
    /// <returns>Colección de roles del usuario</returns>
    IEnumerable<string> GetRolesFromToken(string token);

    /// <summary>
    /// Extrae los permisos de un token
    /// </summary>
    /// <param name="token">Token del cual extraer los permisos</param>
    /// <returns>Colección de permisos del usuario</returns>
    IEnumerable<string> GetPermissionsFromToken(string token);
}