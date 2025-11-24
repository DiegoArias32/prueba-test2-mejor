using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Security;

/// <summary>
/// Representa la relación entre un rol, un formulario y sus permisos
/// </summary>
public class RolFormPermi : BaseEntity
{
    /// <summary>
    /// Identificador del rol
    /// </summary>
    public int RolId { get; set; }

    /// <summary>
    /// Identificador del formulario
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// Identificador de los permisos
    /// </summary>
    public int PermissionId { get; set; }

    /// <summary>
    /// Navegación al rol asociado
    /// </summary>
    public virtual Rol Rol { get; set; } = null!;

    /// <summary>
    /// Navegación al formulario asociado
    /// </summary>
    public virtual Form Form { get; set; } = null!;

    /// <summary>
    /// Navegación a los permisos asociados
    /// </summary>
    public virtual Permission Permission { get; set; } = null!;
}