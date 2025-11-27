using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Security;

/// <summary>
/// Representa un usuario del sistema
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Nombre de usuario para inicio de sesión
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del usuario
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña cifrada del usuario
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Tipo de identificación (CC, TI, CE, PP, NIT)
    /// </summary>
    public string? IdentificationType { get; set; }

    /// <summary>
    /// Número de identificación
    /// </summary>
    public string? IdentificationNumber { get; set; }

    /// <summary>
    /// Número de teléfono de contacto
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Dirección física
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Pestañas permitidas para el usuario en formato JSON o cadena delimitada
    /// </summary>
    public string? AllowedTabs { get; set; }

    /// <summary>
    /// Colección de roles asignados al usuario
    /// </summary>
    public virtual ICollection<RolUser> RolUsers { get; set; } = new List<RolUser>();
}