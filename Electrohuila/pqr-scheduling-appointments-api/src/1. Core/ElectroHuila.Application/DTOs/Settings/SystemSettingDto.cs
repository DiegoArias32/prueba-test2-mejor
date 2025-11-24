namespace ElectroHuila.Application.DTOs.Settings;

/// <summary>
/// DTO para configuración del sistema
/// </summary>
public record SystemSettingDto
{
    public int Id { get; init; }
    public string SettingKey { get; init; } = string.Empty;
    public string? SettingValue { get; init; }
    public string SettingType { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsEncrypted { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO para crear una nueva configuración del sistema
/// </summary>
public record CreateSystemSettingDto
{
    public string SettingKey { get; init; } = string.Empty;
    public string? SettingValue { get; init; }
    public string SettingType { get; init; } = "STRING";
    public string? Description { get; init; }
    public bool IsEncrypted { get; init; }
}

/// <summary>
/// DTO para actualizar una configuración del sistema
/// </summary>
public record UpdateSystemSettingDto
{
    public int Id { get; init; }
    public string? SettingValue { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// DTO para actualizar solo el valor de una configuración
/// </summary>
public record UpdateSystemSettingValueDto
{
    public string SettingKey { get; init; } = string.Empty;
    public string SettingValue { get; init; } = string.Empty;
}
