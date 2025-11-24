using System.ComponentModel.DataAnnotations;

namespace ElectroHuila.Application.DTOs.Assignments;

/// <summary>
/// DTO para asignar múltiples tipos de cita a un usuario
/// </summary>
public class BulkAssignmentDto
{
    /// <summary>
    /// ID del usuario a asignar
    /// </summary>
    [Required(ErrorMessage = "El ID del usuario es requerido")]
    public int UserId { get; set; }

    /// <summary>
    /// Lista de IDs de tipos de cita a asignar
    /// </summary>
    [Required(ErrorMessage = "Debe proporcionar al menos un tipo de cita")]
    [MinLength(1, ErrorMessage = "Debe proporcionar al menos un tipo de cita")]
    public List<int> AppointmentTypeIds { get; set; } = new();
}
