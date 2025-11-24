using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Security;

/// <summary>
/// Representa un módulo del sistema
/// </summary>
public class Module : BaseEntity
{
    /// <summary>
    /// Nombre del módulo
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Código único del módulo
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Colección de formularios asociados a este módulo
    /// </summary>
    public virtual ICollection<FormModule> FormModules { get; set; } = new List<FormModule>();
}