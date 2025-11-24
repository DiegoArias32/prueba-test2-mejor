namespace ElectroHuila.Application.DTOs.Catalogs;

/// <summary>
/// DTO para el estado de una cita
/// </summary>
public record AppointmentStatusDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ColorPrimary { get; init; }
    public string? ColorSecondary { get; init; }
    public string? ColorText { get; init; }
    public string? IconName { get; init; }
    public int DisplayOrder { get; init; }
    public bool AllowCancellation { get; init; }
    public bool IsFinalState { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO para crear un nuevo estado de cita
/// </summary>
public record CreateAppointmentStatusDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ColorPrimary { get; init; }
    public string? ColorSecondary { get; init; }
    public string? ColorText { get; init; }
    public string? IconName { get; init; }
    public int DisplayOrder { get; init; }
    public bool AllowCancellation { get; init; } = true;
    public bool IsFinalState { get; init; }
}

/// <summary>
/// DTO para actualizar un estado de cita existente
/// </summary>
public record UpdateAppointmentStatusDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ColorPrimary { get; init; }
    public string? ColorSecondary { get; init; }
    public string? ColorText { get; init; }
    public string? IconName { get; init; }
    public int DisplayOrder { get; init; }
    public bool AllowCancellation { get; init; }
    public bool IsFinalState { get; init; }
}
