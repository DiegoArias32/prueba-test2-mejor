namespace ElectroHuila.Application.DTOs.Catalogs;

/// <summary>
/// DTO para tipo de documento
/// </summary>
public record DocumentTypeDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}
