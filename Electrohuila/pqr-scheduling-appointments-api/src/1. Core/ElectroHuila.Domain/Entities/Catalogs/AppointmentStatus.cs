using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Catalogs;

/// <summary>
/// Representa los diferentes estados que puede tener una cita
/// </summary>
public class AppointmentStatus : BaseEntity
{
    /// <summary>
    /// Código único del estado (ej: PENDING, COMPLETED, CANCELLED)
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre descriptivo del estado
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del estado
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Color principal para mostrar en el frontend (formato hexadecimal)
    /// </summary>
    public string? ColorPrimary { get; private set; }

    /// <summary>
    /// Color secundario para mostrar en el frontend
    /// </summary>
    public string? ColorSecondary { get; private set; }

    /// <summary>
    /// Color del texto que se muestra sobre el color primario
    /// </summary>
    public string? ColorText { get; private set; }

    /// <summary>
    /// Nombre del icono para mostrar en el frontend (ej: FaClock, FaCheck)
    /// </summary>
    public string? IconName { get; private set; }

    /// <summary>
    /// Orden de visualización en listas
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Indica si se permite cancelar una cita en este estado
    /// </summary>
    public bool AllowCancellation { get; private set; } = true;

    /// <summary>
    /// Indica si este estado es final (no permite transiciones)
    /// </summary>
    public bool IsFinalState { get; private set; }

    // Navegación
    public ICollection<Appointments.Appointment> Appointments { get; private set; } = new List<Appointments.Appointment>();

    private AppointmentStatus() { } // Para EF Core

    public static AppointmentStatus Create(
        string code,
        string name,
        string? description = null,
        string? colorPrimary = null,
        string? colorSecondary = null,
        string? colorText = null,
        string? iconName = null,
        int displayOrder = 0,
        bool allowCancellation = true,
        bool isFinalState = false)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be null or empty", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        return new AppointmentStatus
        {
            Code = code.ToUpperInvariant(),
            Name = name,
            Description = description,
            ColorPrimary = colorPrimary,
            ColorSecondary = colorSecondary,
            ColorText = colorText,
            IconName = iconName,
            DisplayOrder = displayOrder,
            AllowCancellation = allowCancellation,
            IsFinalState = isFinalState,
            IsActive = true
        };
    }

    public void UpdateColors(string colorPrimary, string? colorSecondary = null, string? colorText = null)
    {
        ColorPrimary = colorPrimary;
        ColorSecondary = colorSecondary;
        ColorText = colorText;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateIcon(string iconName)
    {
        IconName = iconName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string? description)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfigureTransitions(bool allowCancellation, bool isFinalState)
    {
        AllowCancellation = allowCancellation;
        IsFinalState = isFinalState;
        UpdatedAt = DateTime.UtcNow;
    }
}
