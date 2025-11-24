namespace ElectroHuila.Application.DTOs.Appointments;

/// <summary>
/// Data transfer object con detalles completos de una cita, incluyendo información relacionada.
/// Usado para evitar el problema de "Unknown" en el frontend.
/// </summary>
public class AppointmentDetailDto
{
    /// <summary>
    /// ID único de la cita
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Número único de la cita
    /// </summary>
    public string AppointmentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de la cita
    /// </summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// Hora de la cita
    /// </summary>
    public string? AppointmentTime { get; set; }

    /// <summary>
    /// Estado de la cita
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Notas adicionales
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Fecha de completado
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Razón de cancelación
    /// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Datos resumidos del cliente
    /// </summary>
    public ClientSummaryDto Client { get; set; } = new();

    /// <summary>
    /// Datos resumidos de la sede
    /// </summary>
    public BranchSummaryDto Branch { get; set; } = new();

    /// <summary>
    /// Datos resumidos del tipo de cita
    /// </summary>
    public AppointmentTypeSummaryDto AppointmentType { get; set; } = new();

    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha de actualización
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indica si está activo
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Datos resumidos de un cliente para DTOs
/// </summary>
public class ClientSummaryDto
{
    /// <summary>
    /// ID del cliente
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Número de cliente
    /// </summary>
    public string? ClientNumber { get; set; }

    /// <summary>
    /// Nombre completo del cliente
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email del cliente
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Teléfono del cliente
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Número de documento
    /// </summary>
    public string DocumentNumber { get; set; } = string.Empty;
}

/// <summary>
/// Datos resumidos de una sede para DTOs
/// </summary>
public class BranchSummaryDto
{
    /// <summary>
    /// ID de la sede
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Código de la sede
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la sede
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Dirección de la sede
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Ciudad de la sede
    /// </summary>
    public string? City { get; set; }
}

/// <summary>
/// Datos resumidos de un tipo de cita para DTOs
/// </summary>
public class AppointmentTypeSummaryDto
{
    /// <summary>
    /// ID del tipo de cita
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Código del tipo de cita
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del tipo de cita
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del tipo de cita
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Nombre del ícono
    /// </summary>
    public string? IconName { get; set; }

    /// <summary>
    /// Color primario
    /// </summary>
    public string? ColorPrimary { get; set; }

    /// <summary>
    /// Color secundario
    /// </summary>
    public string? ColorSecondary { get; set; }
}
