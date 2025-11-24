using System.ComponentModel.DataAnnotations;

namespace ElectroHuila.Application.DTOs.Assignments;

/// <summary>
/// DTO para crear una nueva asignación de usuario a tipo de cita
/// </summary>
public class CreateAssignmentDto
{
    /// <summary>
    /// ID del usuario a asignar
    /// </summary>
    [Required(ErrorMessage = "El ID del usuario es requerido")]
    public int UserId { get; set; }

    /// <summary>
    /// ID del tipo de cita a asignar
    /// </summary>
    [Required(ErrorMessage = "El ID del tipo de cita es requerido")]
    public int AppointmentTypeId { get; set; }
}
