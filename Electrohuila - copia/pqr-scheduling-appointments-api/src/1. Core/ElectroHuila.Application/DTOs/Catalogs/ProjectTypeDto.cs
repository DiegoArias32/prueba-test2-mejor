namespace ElectroHuila.Application.DTOs.Catalogs;

/// <summary>
/// DTO para tipo de proyecto
/// </summary>
public record ProjectTypeDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? IconName { get; init; }
    public string? ColorPrimary { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO para crear un nuevo tipo de proyecto
/// </summary>
public record CreateProjectTypeDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? IconName { get; init; }
    public string? ColorPrimary { get; init; }
    public int DisplayOrder { get; init; }
}

/// <summary>
/// DTO para actualizar un tipo de proyecto existente
/// </summary>
public record UpdateProjectTypeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? IconName { get; init; }
    public string? ColorPrimary { get; init; }
    public int DisplayOrder { get; init; }
}
