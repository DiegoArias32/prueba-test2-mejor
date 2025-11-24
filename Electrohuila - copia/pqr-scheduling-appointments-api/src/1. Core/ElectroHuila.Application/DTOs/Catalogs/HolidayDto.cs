namespace ElectroHuila.Application.DTOs.Catalogs;

/// <summary>
/// DTO para festivo o día no laborable
/// </summary>
public record HolidayDto
{
    public int Id { get; init; }
    public DateTime HolidayDate { get; init; }
    public string HolidayName { get; init; } = string.Empty;
    public string HolidayType { get; init; } = string.Empty;
    public int? BranchId { get; init; }
    public string? BranchName { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO para crear un festivo nacional
/// </summary>
public record CreateNationalHolidayDto
{
    public DateTime HolidayDate { get; init; }
    public string HolidayName { get; init; } = string.Empty;
}

/// <summary>
/// DTO para crear un festivo local
/// </summary>
public record CreateLocalHolidayDto
{
    public DateTime HolidayDate { get; init; }
    public string HolidayName { get; init; } = string.Empty;
    public int BranchId { get; init; }
}

/// <summary>
/// DTO para crear un festivo de empresa
/// </summary>
public record CreateCompanyHolidayDto
{
    public DateTime HolidayDate { get; init; }
    public string HolidayName { get; init; } = string.Empty;
}

/// <summary>
/// DTO para actualizar un festivo
/// </summary>
public record UpdateHolidayDto
{
    public int Id { get; init; }
    public string HolidayName { get; init; } = string.Empty;
    public DateTime? HolidayDate { get; init; }
}

/// <summary>
/// DTO para verificar si una fecha es festivo
/// </summary>
public record CheckHolidayDto
{
    public DateTime Date { get; init; }
    public int? BranchId { get; init; }
}

/// <summary>
/// DTO con el resultado de verificar si una fecha es festivo
/// </summary>
public record HolidayCheckResultDto
{
    public bool IsHoliday { get; init; }
    public string? HolidayName { get; init; }
    public string? HolidayType { get; init; }
}
