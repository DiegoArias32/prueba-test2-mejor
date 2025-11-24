using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Security;

/// <summary>
/// Representa un rol de usuario en el sistema
/// </summary>
public class Rol : BaseEntity
{
    /// <summary>
    /// Nombre del rol
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Código único del rol
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Colección de usuarios asignados a este rol
    /// </summary>
    public virtual ICollection<RolUser> RolUsers { get; set; } = new List<RolUser>();

    /// <summary>
    /// Colección de permisos de formularios asignados a este rol
    /// </summary>
    public virtual ICollection<RolFormPermi> RolFormPermis { get; set; } = new List<RolFormPermi>();
}