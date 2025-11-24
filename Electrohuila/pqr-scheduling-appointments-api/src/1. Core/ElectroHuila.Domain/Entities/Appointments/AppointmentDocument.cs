using ElectroHuila.Domain.Entities.Common;
using ElectroHuila.Domain.Entities.Security;

namespace ElectroHuila.Domain.Entities.Appointments;

/// <summary>
/// Documento adjunto a una cita
/// Permite subir archivos relacionados con citas (cédulas, planos, fotos, etc.)
/// </summary>
public class AppointmentDocument : BaseEntity
{
    /// <summary>
    /// ID de la cita a la que pertenece el documento
    /// </summary>
    public int AppointmentId { get; private set; }

    /// <summary>
    /// Nombre del archivo
    /// Ejemplo: "Cedula_Juan_Perez.pdf"
    /// </summary>
    public string DocumentName { get; private set; } = string.Empty;

    /// <summary>
    /// Tipo de archivo
    /// Ejemplo: "PDF", "JPG", "PNG", "DOCX"
    /// </summary>
    public string? DocumentType { get; private set; }

    /// <summary>
    /// Ruta del archivo en el storage
    /// Ejemplo: "/uploads/2025/11/cedula_123.pdf"
    /// </summary>
    public string FilePath { get; private set; } = string.Empty;

    /// <summary>
    /// Tamaño del archivo en bytes
    /// </summary>
    public long? FileSize { get; private set; }

    /// <summary>
    /// ID del usuario que subió el archivo
    /// </summary>
    public int? UploadedBy { get; private set; }

    /// <summary>
    /// Descripción del documento
    /// Ejemplo: "Cédula de ciudadanía del solicitante"
    /// </summary>
    public string? Description { get; private set; }

    // Navigation properties
    public virtual Appointment Appointment { get; private set; } = null!;
    public virtual User? UploadedByUser { get; private set; }

    private AppointmentDocument() { } // Para EF Core

    public static AppointmentDocument Create(
        int appointmentId,
        string documentName,
        string filePath,
        string? documentType = null,
        long? fileSize = null,
        int? uploadedBy = null,
        string? description = null)
    {
        if (appointmentId <= 0)
            throw new ArgumentException("Appointment ID must be greater than zero", nameof(appointmentId));

        if (string.IsNullOrWhiteSpace(documentName))
            throw new ArgumentException("Document name cannot be null or empty", nameof(documentName));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        return new AppointmentDocument
        {
            AppointmentId = appointmentId,
            DocumentName = documentName,
            DocumentType = documentType?.ToUpperInvariant(),
            FilePath = filePath,
            FileSize = fileSize,
            UploadedBy = uploadedBy,
            Description = description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFilePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        FilePath = filePath;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene la extensión del archivo
    /// </summary>
    public string GetFileExtension()
    {
        return Path.GetExtension(DocumentName);
    }

    /// <summary>
    /// Verifica si el archivo es una imagen
    /// </summary>
    public bool IsImage()
    {
        var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        return imageExtensions.Contains(GetFileExtension().ToLowerInvariant());
    }

    /// <summary>
    /// Verifica si el archivo es un PDF
    /// </summary>
    public bool IsPdf()
    {
        return GetFileExtension().ToLowerInvariant() == ".pdf";
    }

    /// <summary>
    /// Obtiene el tamaño del archivo en formato legible
    /// </summary>
    public string GetFileSizeFormatted()
    {
        if (!FileSize.HasValue)
            return "Desconocido";

        var bytes = FileSize.Value;

        if (bytes < 1024)
            return $"{bytes} B";

        if (bytes < 1024 * 1024)
            return $"{bytes / 1024.0:F2} KB";

        if (bytes < 1024 * 1024 * 1024)
            return $"{bytes / (1024.0 * 1024.0):F2} MB";

        return $"{bytes / (1024.0 * 1024.0 * 1024.0):F2} GB";
    }
}
