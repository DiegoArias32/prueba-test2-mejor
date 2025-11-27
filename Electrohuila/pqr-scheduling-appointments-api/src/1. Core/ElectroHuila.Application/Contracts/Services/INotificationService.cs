namespace ElectroHuila.Application.Contracts.Services;

/// <summary>
/// Servicio para envío de notificaciones de citas
/// Utiliza APIs externas para Email (Gmail API) y WhatsApp (WhatsApp API)
/// Las plantillas están hardcoded en los servicios externos
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Envía un email de confirmación de cita
    /// </summary>
    Task SendAppointmentConfirmationAsync(int appointmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía un recordatorio de cita (24h antes)
    /// </summary>
    Task SendAppointmentReminderAsync(int appointmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía una notificación de cancelación de cita
    /// </summary>
    Task SendAppointmentCancellationAsync(int appointmentId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía una notificación de cita completada
    /// </summary>
    Task SendAppointmentCompletedAsync(int appointmentId, CancellationToken cancellationToken = default);
}
