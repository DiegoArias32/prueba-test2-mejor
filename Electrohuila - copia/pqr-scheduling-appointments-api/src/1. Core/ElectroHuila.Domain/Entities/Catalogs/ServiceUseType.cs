using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Catalogs;

/// <summary>
/// Representa los tipos de uso del servicio (Residencial, Comercial, Industrial, etc.)
/// </summary>
public class ServiceUseType : BaseEntity
{
    /// <summary>
    /// Código único del tipo de uso
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre del tipo de uso
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Orden de visualización
    /// </summary>
    public int DisplayOrder { get; private set; }

    private ServiceUseType() { } // Para EF Core

    public static ServiceUseType Create(
        string code,
        string name,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be null or empty", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        return new ServiceUseType
        {
            Code = code.ToUpperInvariant(),
            Name = name,
            DisplayOrder = displayOrder,
            IsActive = true
        };
    }

    public void UpdateDetails(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        UpdatedAt = DateTime.UtcNow;
    }
}
