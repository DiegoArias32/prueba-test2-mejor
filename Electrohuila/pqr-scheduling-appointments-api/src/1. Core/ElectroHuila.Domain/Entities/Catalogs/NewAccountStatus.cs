using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Catalogs;

/// <summary>
/// Representa los estados posibles para las solicitudes de nuevas cuentas
/// </summary>
public class NewAccountStatus : BaseEntity
{
    /// <summary>
    /// Código único del estado
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre del estado
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción del estado
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Nombre del icono para el frontend
    /// </summary>
    public string? IconName { get; private set; }

    /// <summary>
    /// Color principal para el frontend
    /// </summary>
    public string? ColorPrimary { get; private set; }

    /// <summary>
    /// Orden de visualización
    /// </summary>
    public int DisplayOrder { get; private set; }

    private NewAccountStatus() { } // Para EF Core

    public static NewAccountStatus Create(
        string code,
        string name,
        string? description = null,
        string? iconName = null,
        string? colorPrimary = null,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be null or empty", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        return new NewAccountStatus
        {
            Code = code.ToUpperInvariant(),
            Name = name,
            Description = description,
            IconName = iconName,
            ColorPrimary = colorPrimary,
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

    public void UpdateDesign(string? iconName, string? colorPrimary, int displayOrder)
    {
        IconName = iconName;
        ColorPrimary = colorPrimary;
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
