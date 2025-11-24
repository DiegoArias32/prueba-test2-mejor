using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Security;

/// <summary>
/// Representa un formulario o vista del sistema
/// </summary>
public class Form : BaseEntity
{
    /// <summary>
    /// Nombre del formulario
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Código único del formulario
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Colección de módulos asociados a este formulario
    /// </summary>
    public virtual ICollection<FormModule> FormModules { get; set; } = new List<FormModule>();

    /// <summary>
    /// Colección de permisos de roles asociados a este formulario
    /// </summary>
    public virtual ICollection<RolFormPermi> RolFormPermis { get; set; } = new List<RolFormPermi>();
}