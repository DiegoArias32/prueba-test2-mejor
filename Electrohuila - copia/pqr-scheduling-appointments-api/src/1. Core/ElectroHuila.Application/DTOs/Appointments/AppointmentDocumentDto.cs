namespace ElectroHuila.Application.DTOs.Appointments;

/// <summary>
/// DTO para documento adjunto a cita
/// </summary>
public record AppointmentDocumentDto
{
    public int Id { get; init; }
    public int AppointmentId { get; init; }
    public string DocumentName { get; init; } = string.Empty;
    public string? DocumentType { get; init; }
    public string FilePath { get; init; } = string.Empty;
    public long? FileSize { get; init; }
    public string FileSizeFormatted { get; init; } = string.Empty;
    public int? UploadedBy { get; init; }
    public string? UploadedByName { get; init; }
    public string? Description { get; init; }
    public bool IsImage { get; init; }
    public bool IsPdf { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO para crear un documento adjunto
/// </summary>
public record CreateAppointmentDocumentDto
{
    public int AppointmentId { get; init; }
    public string DocumentName { get; init; } = string.Empty;
    public string FilePath { get; init; } = string.Empty;
    public string? DocumentType { get; init; }
    public long? FileSize { get; init; }
    public int? UploadedBy { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// DTO para actualizar la descripción de un documento
/// </summary>
public record UpdateAppointmentDocumentDto
{
    public int Id { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// DTO para subir un archivo
/// </summary>
public record UploadDocumentDto
{
    public int AppointmentId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string Base64Content { get; init; } = string.Empty;
    public string? Description { get; init; }
}

/// <summary>
/// DTO con estadísticas de documentos de una cita
/// </summary>
public record AppointmentDocumentsStatsDto
{
    public int AppointmentId { get; init; }
    public int TotalDocuments { get; init; }
    public long TotalSizeBytes { get; init; }
    public string TotalSizeFormatted { get; init; } = string.Empty;
    public int ImageCount { get; init; }
    public int PdfCount { get; init; }
    public int OtherCount { get; init; }
}
