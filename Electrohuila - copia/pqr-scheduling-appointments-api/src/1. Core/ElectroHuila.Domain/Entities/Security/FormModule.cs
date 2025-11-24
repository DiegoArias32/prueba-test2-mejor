using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Security;

/// <summary>
/// Representa la relación entre un formulario y un módulo
/// </summary>
public class FormModule : BaseEntity
{
    /// <summary>
    /// Identificador del formulario
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// Identificador del módulo
    /// </summary>
    public int ModuleId { get; set; }

    /// <summary>
    /// Navegación al formulario asociado
    /// </summary>
    public virtual Form Form { get; set; } = null!;

    /// <summary>
    /// Navegación al módulo asociado
    /// </summary>
    public virtual Module Module { get; set; } = null!;
}