namespace ElectroHuila.Application.DTOs.Catalogs;

/// <summary>
/// DTO para tipo de uso de servicio
/// </summary>
public record ServiceUseTypeDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO para crear un nuevo tipo de uso de servicio
/// </summary>
public record CreateServiceUseTypeDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
}

/// <summary>
/// DTO para actualizar un tipo de uso de servicio existente
/// </summary>
public record UpdateServiceUseTypeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
}
