namespace ElectroHuila.Application.DTOs.Assignments;

/// <summary>
/// Data transfer object para la asignación de usuario a tipo de cita
/// </summary>
public class UserAssignmentDto
{
    /// <summary>
    /// ID de la asignación
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID del usuario
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Nombre de usuario
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email del usuario
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// ID del tipo de cita
    /// </summary>
    public int AppointmentTypeId { get; set; }

    /// <summary>
    /// Código del tipo de cita
    /// </summary>
    public string AppointmentTypeCode { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del tipo de cita
    /// </summary>
    public string AppointmentTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del tipo de cita
    /// </summary>
    public string? AppointmentTypeDescription { get; set; }

    /// <summary>
    /// Nombre del ícono del tipo de cita
    /// </summary>
    public string? AppointmentTypeIcon { get; set; }

    /// <summary>
    /// Color primario del tipo de cita
    /// </summary>
    public string? AppointmentTypeColor { get; set; }

    /// <summary>
    /// Fecha de creación de la asignación
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha de última actualización
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indica si la asignación está activa
    /// </summary>
    public bool IsActive { get; set; }
}
