using System.Security.Claims;

namespace ElectroHuila.Application.Contracts.Services;

/// <summary>
/// Servicio para validación de tokens de autenticación
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Valida un token y extrae el ClaimsPrincipal
    /// </summary>
    /// <param name="token">Token a validar</param>
    /// <returns>ClaimsPrincipal con la información del usuario o null si el token es inválido</returns>
    ClaimsPrincipal? ValidateToken(string token);
}
