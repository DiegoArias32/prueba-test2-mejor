using ElectroHuila.Application.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ElectroHuila.Infrastructure.Identity;

/// <summary>
/// Servicio para validación de tokens JWT y extracción de claims de usuario.
/// Implementa <see cref="ITokenService"/> para proporcionar funcionalidades de validación de tokens.
/// </summary>
/// <remarks>
/// Este servicio se enfoca específicamente en la validación de tokens JWT y la extracción
/// de información del usuario (ClaimsPrincipal) a partir de tokens válidos.
/// Utiliza la configuración de la aplicación para obtener los parámetros de validación JWT.
/// </remarks>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="TokenService"/>.
    /// </summary>
    /// <param name="configuration">Configuración de la aplicación que contiene los parámetros JWT.</param>
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Valida un token JWT y extrae el ClaimsPrincipal del usuario si el token es válido.
    /// </summary>
    /// <param name="token">Token JWT a validar como string.</param>
    /// <returns>
    /// <see cref="ClaimsPrincipal"/> que contiene los claims del usuario si el token es válido;
    /// null si el token es inválido o ha expirado.
    /// </returns>
    /// <remarks>
    /// El proceso de validación incluye:
    /// 1. Verificación de la firma del token usando la clave secreta
    /// 2. Validación del emisor (Issuer) configurado
    /// 3. Validación de la audiencia (Audience) configurada
    /// 4. Verificación del tiempo de vida del token
    /// 5. ClockSkew establecido en cero para validación exacta de tiempo
    /// 
    /// Si cualquiera de estas validaciones falla, el método retorna null.
    /// </remarks>
    /// <example>
    /// <code>
    /// var tokenService = new TokenService(configuration);
    /// var principal = tokenService.ValidateToken("eyJhbGciOiJIUzI1NiIs...");
    /// if (principal != null)
    /// {
    ///     var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    ///     var username = principal.FindFirst(ClaimTypes.Name)?.Value;
    /// }
    /// </code>
    /// </example>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
