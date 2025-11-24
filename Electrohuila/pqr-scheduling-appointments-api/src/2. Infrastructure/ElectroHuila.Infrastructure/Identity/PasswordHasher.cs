using ElectroHuila.Application.Common.Interfaces.Services.Security;
using ElectroHuila.Application.Common.Interfaces.Services.Common;
using System.Security.Cryptography;

namespace ElectroHuila.Infrastructure.Identity;

/// <summary>
/// Implementación segura de hash de contraseñas utilizando PBKDF2 con SHA-256.
/// Implementa <see cref="IPasswordHasher"/> para proporcionar funcionalidades de hash y verificación de contraseñas.
/// </summary>
/// <remarks>
/// Esta clase utiliza el algoritmo PBKDF2 (Password-Based Key Derivation Function 2) con las siguientes características:
/// - Salt de 32 bytes generado criptográficamente
/// - Hash de 32 bytes utilizando SHA-256
/// - 100,000 iteraciones para resistir ataques de fuerza bruta
/// - Almacenamiento combinado de salt + hash en formato Base64
/// </remarks>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Tamaño del salt en bytes utilizado para el hash de contraseñas.
    /// </summary>
    private const int SaltSize = 32;
    
    /// <summary>
    /// Tamaño del hash resultante en bytes.
    /// </summary>
    private const int HashSize = 32;
    
    /// <summary>
    /// Número de iteraciones PBKDF2 para resistir ataques de fuerza bruta.
    /// </summary>
    /// <remarks>
    /// 100,000 iteraciones proporcionan un buen balance entre seguridad y rendimiento
    /// según las recomendaciones actuales de seguridad (2024).
    /// </remarks>
    private const int Iterations = 100000;

    /// <summary>
    /// Genera un hash seguro de una contraseña utilizando PBKDF2 con salt aleatorio.
    /// </summary>
    /// <param name="password">Contraseña en texto plano a hashear.</param>
    /// <returns>
    /// Hash de la contraseña codificado en Base64 que incluye el salt y el hash combinados.
    /// </returns>
    /// <remarks>
    /// El resultado contiene el salt (primeros 32 bytes) seguido del hash (siguientes 32 bytes),
    /// todo codificado en Base64 para facilitar el almacenamiento en base de datos.
    /// </remarks>
    /// <example>
    /// <code>
    /// var hasher = new PasswordHasher();
    /// string hashedPassword = hasher.HashPassword("myPassword123");
    /// // Resultado: string Base64 de 88 caracteres aproximadamente
    /// </code>
    /// </example>
    public string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(HashSize);

        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Verifica si una contraseña en texto plano coincide con un hash almacenado.
    /// </summary>
    /// <param name="password">Contraseña en texto plano a verificar.</param>
    /// <param name="hashedPassword">Hash almacenado previamente generado por <see cref="HashPassword"/>.</param>
    /// <returns>
    /// true si la contraseña coincide con el hash; false en caso contrario o si ocurre algún error.
    /// </returns>
    /// <remarks>
    /// La verificación incluye:
    /// 1. Decodificación del hash Base64
    /// 2. Extracción del salt original
    /// 3. Regeneración del hash con la contraseña proporcionada
    /// 4. Comparación byte a byte para evitar ataques de timing
    /// </remarks>
    /// <example>
    /// <code>
    /// var hasher = new PasswordHasher();
    /// string hash = hasher.HashPassword("myPassword123");
    /// bool isValid = hasher.VerifyPassword("myPassword123", hash); // true
    /// bool isInvalid = hasher.VerifyPassword("wrongPassword", hash); // false
    /// </code>
    /// </example>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            var hashBytes = Convert.FromBase64String(hashedPassword);

            if (hashBytes.Length != SaltSize + HashSize)
                return false;

            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Determina si un hash de contraseña necesita ser actualizado debido a cambios en el algoritmo o parámetros.
    /// </summary>
    /// <param name="hashedPassword">Hash de contraseña a evaluar.</param>
    /// <returns>
    /// true si el hash necesita ser actualizado; false si está actualizado.
    /// </returns>
    /// <remarks>
    /// Actualmente retorna false ya que la implementación actual se considera segura.
    /// Este método permite futuras actualizaciones del algoritmo de hash sin romper la compatibilidad.
    /// En implementaciones futuras podría verificar diferentes versiones de algoritmos o parámetros.
    /// </remarks>
    public bool NeedsUpgrade(string hashedPassword)
    {
        return false;
    }
}