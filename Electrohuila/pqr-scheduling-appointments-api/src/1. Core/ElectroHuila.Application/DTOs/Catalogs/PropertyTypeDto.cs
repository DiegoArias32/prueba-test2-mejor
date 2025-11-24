namespace ElectroHuila.Application.DTOs.Catalogs;

/// <summary>
/// DTO para tipo de propiedad
/// </summary>
public record PropertyTypeDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? IconName { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO para crear un nuevo tipo de propiedad
/// </summary>
public record CreatePropertyTypeDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? IconName { get; init; }
    public int DisplayOrder { get; init; }
}

/// <summary>
/// DTO para actualizar un tipo de propiedad existente
/// </summary>
public record UpdatePropertyTypeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? IconName { get; init; }
    public int DisplayOrder { get; init; }
}
