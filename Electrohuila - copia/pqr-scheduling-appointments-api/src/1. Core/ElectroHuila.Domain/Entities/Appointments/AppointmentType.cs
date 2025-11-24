using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Appointments;

/// <summary>
/// Representa un tipo de cita disponible en el sistema
/// </summary>
public class AppointmentType : BaseEntity
{
    /// <summary>
    /// Código único del tipo de cita
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre del tipo de cita
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del tipo de cita
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Nombre del icono para el frontend (ej: FaPlug, FaBolt)
    /// </summary>
    public string? IconName { get; private set; }

    /// <summary>
    /// Color principal para el frontend (formato hexadecimal)
    /// </summary>
    public string? ColorPrimary { get; private set; }

    /// <summary>
    /// Color secundario para el frontend
    /// </summary>
    public string? ColorSecondary { get; private set; }

    /// <summary>
    /// Tiempo estimado en minutos que toma este tipo de cita
    /// </summary>
    public int EstimatedTimeMinutes { get; private set; } = 120;

    /// <summary>
    /// Indica si este tipo de cita requiere documentación adicional
    /// </summary>
    public bool RequiresDocumentation { get; private set; } = true;

    /// <summary>
    /// Orden de visualización en listas
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Colección de citas asociadas a este tipo
    /// </summary>
    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();

    private AppointmentType() { } // Para EF Core

    public static AppointmentType Create(
        string code,
        string name,
        string? description = null,
        string? iconName = null,
        string? colorPrimary = null,
        string? colorSecondary = null,
        int estimatedTimeMinutes = 120,
        bool requiresDocumentation = true,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be null or empty", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        if (estimatedTimeMinutes <= 0)
            throw new ArgumentException("EstimatedTimeMinutes must be greater than zero", nameof(estimatedTimeMinutes));

        return new AppointmentType
        {
            Code = code.ToUpperInvariant(),
            Name = name,
            Description = description,
            IconName = iconName,
            ColorPrimary = colorPrimary,
            ColorSecondary = colorSecondary,
            EstimatedTimeMinutes = estimatedTimeMinutes,
            RequiresDocumentation = requiresDocumentation,
            DisplayOrder = displayOrder,
            IsActive = true
        };
    }

    public void UpdateDetails(string name, string? description)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDesign(string? iconName, string? colorPrimary, string? colorSecondary)
    {
        IconName = iconName;
        ColorPrimary = colorPrimary;
        ColorSecondary = colorSecondary;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateConfiguration(int estimatedTimeMinutes, bool requiresDocumentation)
    {
        if (estimatedTimeMinutes <= 0)
            throw new ArgumentException("EstimatedTimeMinutes must be greater than zero", nameof(estimatedTimeMinutes));

        EstimatedTimeMinutes = estimatedTimeMinutes;
        RequiresDocumentation = requiresDocumentation;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}