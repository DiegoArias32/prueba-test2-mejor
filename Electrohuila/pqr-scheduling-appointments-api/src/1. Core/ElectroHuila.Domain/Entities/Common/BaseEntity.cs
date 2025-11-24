namespace ElectroHuila.Domain.Entities.Common;

/// <summary>
/// Clase base abstracta para todas las entidades del dominio
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único de la entidad
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Fecha y hora de creación de la entidad en formato UTC
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha y hora de la última actualización de la entidad
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indica si la entidad está activa o ha sido eliminada lógicamente.
    /// El valor por defecto es true (1 en Oracle).
    /// </summary>
    public bool IsActive { get; set; } = true;
}