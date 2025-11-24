using ElectroHuila.Application.Common.Interfaces.Services.Security;
using ElectroHuila.Application.Common.Interfaces.Services.Common;
using ElectroHuila.Domain.Entities.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ElectroHuila.Infrastructure.Identity;

/// <summary>
/// Generador y validador de tokens JWT (JSON Web Tokens) para autenticación y autorización.
/// Implementa <see cref="IJwtTokenGenerator"/> para proporcionar funcionalidades completas de manejo de tokens JWT.
/// </summary>
/// <remarks>
/// Esta clase maneja la creación, validación y extracción de información de tokens JWT.
/// Utiliza configuración externa para establecer parámetros de seguridad como claves secretas,
/// emisor, audiencia y tiempos de expiración. Soporta tanto access tokens como refresh tokens.
/// </remarks>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;
    
    /// <summary>
    /// Clave secreta utilizada para firmar y validar los tokens JWT.
    /// </summary>
    private readonly string _secretKey;
    
    /// <summary>
    /// Emisor del token JWT que identifica quién creó el token.
    /// </summary>
    private readonly string _issuer;
    
    /// <summary>
    /// Audiencia del token JWT que identifica para quién está destinado el token.
    /// </summary>
    private readonly string _audience;
    
    /// <summary>
    /// Tiempo de expiración de los access tokens en horas.
    /// </summary>
    private readonly double _expirationHours;
    
    /// <summary>
    /// Tiempo de expiración de los refresh tokens en días.
    /// </summary>
    private readonly double _refreshExpirationDays;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="JwtTokenGenerator"/>.
    /// </summary>
    /// <param name="configuration">Configuración de la aplicación que contiene los parámetros JWT.</param>
    /// <exception cref="InvalidOperationException">
    /// Se lanza cuando algún parámetro requerido de configuración JWT no está presente.
    /// </exception>
    /// <remarks>
    /// Lee y valida la configuración JWT incluyendo Key, Issuer, Audience y tiempos de expiración.
    /// </remarks>
    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        _issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
        _audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
        _expirationHours = double.Parse(_configuration["Jwt:ExpirationHours"] ?? "1");
        _refreshExpirationDays = double.Parse(_configuration["Jwt:RefreshExpirationDays"] ?? "7");
    }

    /// <summary>
    /// Genera un token JWT de acceso para un usuario con sus roles y permisos específicos.
    /// </summary>
    /// <param name="user">Usuario para el cual se genera el token.</param>
    /// <param name="roles">Colección de roles asignados al usuario.</param>
    /// <param name="permissions">Colección de permisos asignados al usuario.</param>
    /// <returns>Token JWT como string codificado.</returns>
    /// <remarks>
    /// El token incluye claims estándar (NameIdentifier, Name, Email) y claims personalizados
    /// para roles y permisos. Utiliza algoritmo HMAC SHA-256 para la firma.
    /// </remarks>
    public string GenerateToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_expirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Genera un refresh token criptográficamente seguro para renovación de tokens de acceso.
    /// </summary>
    /// <returns>Refresh token como string codificado en Base64.</returns>
    /// <remarks>
    /// Utiliza <see cref="RandomNumberGenerator"/> para generar 64 bytes aleatorios seguros.
    /// El token resultante se codifica en Base64 para facilitar su transmisión.
    /// </remarks>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Valida la integridad y vigencia de un token JWT.
    /// </summary>
    /// <param name="token">Token JWT a validar.</param>
    /// <returns>true si el token es válido; de lo contrario, false.</returns>
    /// <remarks>
    /// Valida la firma, emisor, audiencia y tiempo de vida del token.
    /// ClockSkew se establece en cero para validación exacta de tiempo.
    /// </remarks>
    public bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Obtiene la fecha y hora de expiración de un token JWT.
    /// </summary>
    /// <param name="token">Token JWT del cual extraer la fecha de expiración.</param>
    /// <returns>Fecha y hora de expiración del token en UTC.</returns>
    public DateTime GetTokenExpiration(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.ValidTo;
    }

    /// <summary>
    /// Extrae el identificador de usuario desde un token JWT.
    /// </summary>
    /// <param name="token">Token JWT del cual extraer el ID de usuario.</param>
    /// <returns>ID de usuario como string si existe; de lo contrario, null.</returns>
    /// <remarks>
    /// Busca el claim <see cref="ClaimTypes.NameIdentifier"/> en el token.
    /// </remarks>
    public string? GetUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Extrae el nombre de usuario desde un token JWT.
    /// </summary>
    /// <param name="token">Token JWT del cual extraer el nombre de usuario.</param>
    /// <returns>Nombre de usuario como string si existe; de lo contrario, null.</returns>
    /// <remarks>
    /// Busca el claim <see cref="ClaimTypes.Name"/> en el token.
    /// </remarks>
    public string? GetUsernameFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
    }

    /// <summary>
    /// Extrae todos los roles de usuario desde un token JWT.
    /// </summary>
    /// <param name="token">Token JWT del cual extraer los roles.</param>
    /// <returns>Colección de nombres de roles como strings.</returns>
    /// <remarks>
    /// Busca todos los claims de tipo <see cref="ClaimTypes.Role"/> en el token.
    /// </remarks>
    public IEnumerable<string> GetRolesFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
    }

    /// <summary>
    /// Extrae todos los permisos de usuario desde un token JWT.
    /// </summary>
    /// <param name="token">Token JWT del cual extraer los permisos.</param>
    /// <returns>Colección de nombres de permisos como strings.</returns>
    /// <remarks>
    /// Busca todos los claims de tipo "permission" en el token.
    /// </remarks>
    public IEnumerable<string> GetPermissionsFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.Claims.Where(c => c.Type == "permission").Select(c => c.Value);
    }
}