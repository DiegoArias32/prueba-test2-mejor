using ElectroHuila.Domain.Entities.Catalogs;
using ElectroHuila.Domain.Entities.Common;
using ElectroHuila.Domain.Entities.Clients;
using ElectroHuila.Domain.Entities.Locations;

namespace ElectroHuila.Domain.Entities.Appointments;

/// <summary>
/// Representa una cita programada en el sistema
/// </summary>
public class Appointment : BaseEntity
{
    /// <summary>
    /// Número único de la cita generado por el sistema
    /// </summary>
    public string AppointmentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Fecha programada para la cita
    /// </summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// Hora programada para la cita en formato de texto
    /// </summary>
    public string? AppointmentTime { get; set; }

    /// <summary>
    /// Identificador del estado actual de la cita
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Notas o comentarios adicionales sobre la cita
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Razón de cancelación de la cita (si aplica)
    /// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Fecha y hora en que se completó la cita
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Identificador del cliente asociado a la cita
    /// </summary>
    public int ClientId { get; set; }

    /// <summary>
    /// Identificador de la sucursal donde se realizará la cita
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// Identificador del tipo de cita
    /// </summary>
    public int AppointmentTypeId { get; set; }

    /// <summary>
    /// Indica si la cita está habilitada en el sistema
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Navegación al cliente asociado a la cita
    /// </summary>
    public virtual Client Client { get; set; } = null!;

    /// <summary>
    /// Navegación a la sucursal donde se realizará la cita
    /// </summary>
    public virtual Branch Branch { get; set; } = null!;

    /// <summary>
    /// Navegación al tipo de cita
    /// </summary>
    public virtual AppointmentType AppointmentType { get; set; } = null!;

    /// <summary>
    /// Navegación al estado de la cita
    /// </summary>
    public virtual AppointmentStatus Status { get; set; } = null!;
}