using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Catalogs;

/// <summary>
/// Representa los tipos de propiedad (Casa, Apartamento, Local Comercial, etc.)
/// </summary>
public class PropertyType : BaseEntity
{
    /// <summary>
    /// Código único del tipo de propiedad
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre del tipo de propiedad
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre del icono para el frontend
    /// </summary>
    public string? IconName { get; private set; }

    /// <summary>
    /// Orden de visualización
    /// </summary>
    public int DisplayOrder { get; private set; }

    private PropertyType() { } // Para EF Core

    public static PropertyType Create(
        string code,
        string name,
        string? iconName = null,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be null or empty", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        return new PropertyType
        {
            Code = code.ToUpperInvariant(),
            Name = name,
            IconName = iconName,
            DisplayOrder = displayOrder,
            IsActive = true
        };
    }

    public void UpdateDetails(string name, string? iconName)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        IconName = iconName;
        UpdatedAt = DateTime.UtcNow;
    }
}
