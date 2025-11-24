namespace ElectroHuila.Application.DTOs.Catalogs;

/// <summary>
/// DTO para estado de nueva cuenta
/// </summary>
public record NewAccountStatusDto
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
/// DTO para crear un nuevo estado de nueva cuenta
/// </summary>
public record CreateNewAccountStatusDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? IconName { get; init; }
    public string? ColorPrimary { get; init; }
    public int DisplayOrder { get; init; }
}

/// <summary>
/// DTO para actualizar un estado de nueva cuenta existente
/// </summary>
public record UpdateNewAccountStatusDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? IconName { get; init; }
    public string? ColorPrimary { get; init; }
    public int DisplayOrder { get; init; }
}
