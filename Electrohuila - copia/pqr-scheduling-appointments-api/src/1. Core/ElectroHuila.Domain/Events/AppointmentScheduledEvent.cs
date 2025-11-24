namespace ElectroHuila.Domain.Events;

/// <summary>
/// Evento que se dispara cuando se programa una cita
/// </summary>
public sealed record AppointmentScheduledEvent : DomainEvent
{
    /// <summary>
    /// Tipo de evento
    /// </summary>
    public override string EventType => "AppointmentScheduled";

    /// <summary>
    /// Identificador de la cita
    /// </summary>
    public int AppointmentId { get; init; }

    /// <summary>
    /// Número de la cita
    /// </summary>
    public string AppointmentNumber { get; init; } = string.Empty;

    /// <summary>
    /// Identificador del cliente
    /// </summary>
    public int ClientId { get; init; }

    /// <summary>
    /// Nombre del cliente
    /// </summary>
    public string ClientName { get; init; } = string.Empty;

    /// <summary>
    /// Email del cliente
    /// </summary>
    public string ClientEmail { get; init; } = string.Empty;

    /// <summary>
    /// Identificador de la sucursal
    /// </summary>
    public int BranchId { get; init; }

    /// <summary>
    /// Nombre de la sucursal
    /// </summary>
    public string BranchName { get; init; } = string.Empty;

    /// <summary>
    /// Identificador del tipo de cita
    /// </summary>
    public int AppointmentTypeId { get; init; }

    /// <summary>
    /// Nombre del tipo de cita
    /// </summary>
    public string AppointmentTypeName { get; init; } = string.Empty;

    /// <summary>
    /// Fecha de la cita
    /// </summary>
    public DateTime AppointmentDate { get; init; }

    /// <summary>
    /// Hora de la cita
    /// </summary>
    public string AppointmentTime { get; init; } = string.Empty;

    /// <summary>
    /// Notas adicionales de la cita
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Constructor para crear el evento de cita programada
    /// </summary>
    public AppointmentScheduledEvent(
        int appointmentId,
        string appointmentNumber,
        int clientId,
        string clientName,
        string clientEmail,
        int branchId,
        string branchName,
        int appointmentTypeId,
        string appointmentTypeName,
        DateTime appointmentDate,
        string appointmentTime,
        string? notes = null)
    {
        AppointmentId = appointmentId;
        AppointmentNumber = appointmentNumber;
        ClientId = clientId;
        ClientName = clientName;
        ClientEmail = clientEmail;
        BranchId = branchId;
        BranchName = branchName;
        AppointmentTypeId = appointmentTypeId;
        AppointmentTypeName = appointmentTypeName;
        AppointmentDate = appointmentDate;
        AppointmentTime = appointmentTime;
        Notes = notes;
    }
}