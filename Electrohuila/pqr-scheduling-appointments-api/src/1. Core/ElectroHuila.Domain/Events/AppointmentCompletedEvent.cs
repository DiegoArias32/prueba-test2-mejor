namespace ElectroHuila.Domain.Events;

/// <summary>
/// Evento que se dispara cuando se completa una cita
/// </summary>
public sealed record AppointmentCompletedEvent : DomainEvent
{
    /// <summary>
    /// Tipo de evento
    /// </summary>
    public override string EventType => "AppointmentCompleted";

    /// <summary>
    /// Identificador de la cita completada
    /// </summary>
    public int AppointmentId { get; init; }

    /// <summary>
    /// Número de la cita completada
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
    /// Correo electrónico del cliente
    /// </summary>
    public string ClientEmail { get; init; } = string.Empty;

    /// <summary>
    /// Identificador de la sucursal donde se completó la cita
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
    /// Fecha y hora en que se completó la cita
    /// </summary>
    public DateTime CompletedDate { get; init; }

    /// <summary>
    /// Usuario que marcó la cita como completada
    /// </summary>
    public string CompletedBy { get; init; } = string.Empty;

    /// <summary>
    /// Notas adicionales sobre la finalización de la cita
    /// </summary>
    public string? CompletionNotes { get; init; }

    /// <summary>
    /// Duración real de la cita
    /// </summary>
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// Constructor para crear el evento de cita completada
    /// </summary>
    public AppointmentCompletedEvent(
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
        DateTime completedDate,
        string completedBy,
        string? completionNotes = null,
        TimeSpan duration = default)
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
        CompletedDate = completedDate;
        CompletedBy = completedBy;
        CompletionNotes = completionNotes;
        Duration = duration;
    }
}