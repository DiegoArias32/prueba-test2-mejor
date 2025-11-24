namespace ElectroHuila.Application.Common.Interfaces.Services.Security;

/// <summary>
/// Servicio para hashear y verificar contraseñas de forma segura
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Genera un hash seguro de una contraseña
    /// </summary>
    /// <param name="password">Contraseña en texto plano</param>
    /// <returns>Hash de la contraseña</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifica si una contraseña coincide con un hash
    /// </summary>
    /// <param name="password">Contraseña en texto plano a verificar</param>
    /// <param name="hashedPassword">Hash almacenado de la contraseña</param>
    /// <returns>True si la contraseña es correcta</returns>
    bool VerifyPassword(string password, string hashedPassword);

    /// <summary>
    /// Verifica si un hash necesita ser actualizado con algoritmos más seguros
    /// </summary>
    /// <param name="hashedPassword">Hash a verificar</param>
    /// <returns>True si el hash necesita actualización</returns>
    bool NeedsUpgrade(string hashedPassword);
}