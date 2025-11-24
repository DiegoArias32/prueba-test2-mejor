using ElectroHuila.Domain.Entities.Common;
using ElectroHuila.Domain.Entities.Clients;

namespace ElectroHuila.Domain.Entities.Catalogs;

/// <summary>
/// Representa los diferentes tipos de proyectos (Urbanización, Centro Comercial, etc.)
/// </summary>
public class ProjectType : BaseEntity
{
    /// <summary>
    /// Código único del tipo de proyecto
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre del tipo de proyecto
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción del tipo de proyecto
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

    private ProjectType() { } // Para EF Core

    public static ProjectType Create(
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

        return new ProjectType
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

    public void UpdateDesign(string? iconName, string? colorPrimary)
    {
        IconName = iconName;
        ColorPrimary = colorPrimary;
        UpdatedAt = DateTime.UtcNow;
    }
}
