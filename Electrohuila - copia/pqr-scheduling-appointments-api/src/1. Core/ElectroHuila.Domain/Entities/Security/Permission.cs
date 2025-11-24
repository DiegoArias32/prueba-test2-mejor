using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Security;

/// <summary>
/// Representa los permisos de acceso y operaciones
/// </summary>
public class Permission : BaseEntity
{
    /// <summary>
    /// Indica si tiene permiso de lectura
    /// </summary>
    public bool CanRead { get; set; } = false;

    /// <summary>
    /// Indica si tiene permiso de creación
    /// </summary>
    public bool CanCreate { get; set; } = false;

    /// <summary>
    /// Indica si tiene permiso de actualización
    /// </summary>
    public bool CanUpdate { get; set; } = false;

    /// <summary>
    /// Indica si tiene permiso de eliminación
    /// </summary>
    public bool CanDelete { get; set; } = false;

    /// <summary>
    /// Colección de permisos de roles y formularios asociados
    /// </summary>
    public virtual ICollection<RolFormPermi> RolFormPermis { get; set; } = new List<RolFormPermi>();
}