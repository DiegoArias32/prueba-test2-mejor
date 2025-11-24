using ElectroHuila.Domain.Entities.Common;
using ElectroHuila.Domain.Entities.Security;
using ElectroHuila.Domain.Entities.Appointments;

namespace ElectroHuila.Domain.Entities.Assignments;

/// <summary>
/// Representa la asignación de un usuario a un tipo de cita específico.
/// Permite filtrar qué tipos de citas puede ver y gestionar cada empleado.
/// </summary>
public class UserAppointmentTypeAssignment : BaseEntity
{
    /// <summary>
    /// Identificador del usuario asignado
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Identificador del tipo de cita asignado
    /// </summary>
    public int AppointmentTypeId { get; set; }

    /// <summary>
    /// Navegación al usuario asignado
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Navegación al tipo de cita asignado
    /// </summary>
    public virtual AppointmentType AppointmentType { get; set; } = null!;
}
