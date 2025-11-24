using ElectroHuila.Domain.Entities.Common;
using ElectroHuila.Domain.Entities.Locations;

namespace ElectroHuila.Domain.Entities.Appointments;

/// <summary>
/// Representa un horario disponible para citas en una sucursal
/// </summary>
public class AvailableTime : BaseEntity
{
    /// <summary>
    /// Hora disponible en formato de texto (ej: "09:00", "14:30")
    /// </summary>
    public string Time { get; set; } = string.Empty;

    /// <summary>
    /// Identificador de la sucursal donde está disponible este horario
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// Identificador del tipo de cita al que aplica este horario (opcional)
    /// </summary>
    public int? AppointmentTypeId { get; set; }

    /// <summary>
    /// Navegación a la sucursal donde está disponible este horario
    /// </summary>
    public virtual Branch Branch { get; set; } = null!;

    /// <summary>
    /// Navegación al tipo de cita al que aplica este horario
    /// </summary>
    public virtual AppointmentType? AppointmentType { get; set; }
}