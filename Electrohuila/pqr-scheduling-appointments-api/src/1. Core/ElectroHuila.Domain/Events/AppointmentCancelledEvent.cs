namespace ElectroHuila.Domain.Events;

/// <summary>
/// Evento que se dispara cuando se cancela una cita
/// </summary>
public sealed record AppointmentCancelledEvent : DomainEvent
{
    /// <summary>
    /// Tipo de evento
    /// </summary>
    public override string EventType => "AppointmentCancelled";

    /// <summary>
    /// Identificador de la cita cancelada
    /// </summary>
    public int AppointmentId { get; init; }

    /// <summary>
    /// Número de la cita cancelada
    /// </summary>
    public string AppointmentNumber { get; init; } = string.Empty;

    /// <summary>
    /// Identificador del cliente asociado
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
    /// Fecha original de la cita antes de ser cancelada
    /// </summary>
    public DateTime OriginalAppointmentDate { get; init; }

    /// <summary>
    /// Hora original de la cita antes de ser cancelada
    /// </summary>
    public string OriginalAppointmentTime { get; init; } = string.Empty;

    /// <summary>
    /// Motivo de la cancelación de la cita
    /// </summary>
    public string CancellationReason { get; init; } = string.Empty;

    /// <summary>
    /// Usuario o sistema que canceló la cita
    /// </summary>
    public string CancelledBy { get; init; } = string.Empty;

    /// <summary>
    /// Constructor para crear el evento de cita cancelada
    /// </summary>
    public AppointmentCancelledEvent(
        int appointmentId,
        string appointmentNumber,
        int clientId,
        string clientName,
        string clientEmail,
        DateTime originalAppointmentDate,
        string originalAppointmentTime,
        string cancellationReason,
        string cancelledBy)
    {
        AppointmentId = appointmentId;
        AppointmentNumber = appointmentNumber;
        ClientId = clientId;
        ClientName = clientName;
        ClientEmail = clientEmail;
        OriginalAppointmentDate = originalAppointmentDate;
        OriginalAppointmentTime = originalAppointmentTime;
        CancellationReason = cancellationReason;
        CancelledBy = cancelledBy;
    }
}