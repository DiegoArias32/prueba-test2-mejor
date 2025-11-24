using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Security;

/// <summary>
/// Representa la relación entre un rol y un usuario
/// </summary>
public class RolUser : BaseEntity
{
    /// <summary>
    /// Identificador del rol
    /// </summary>
    public int RolId { get; set; }

    /// <summary>
    /// Identificador del usuario
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Navegación al rol asociado
    /// </summary>
    public virtual Rol Rol { get; set; } = null!;

    /// <summary>
    /// Navegación al usuario asociado
    /// </summary>
    public virtual User User { get; set; } = null!;
}