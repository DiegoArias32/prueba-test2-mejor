namespace ElectroHuila.Application.Contracts.Services;

/// <summary>
/// Servicio para envío de notificaciones (Email, SMS, Push)
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
    /// Envía un email usando una plantilla
    /// </summary>
    Task<bool> SendEmailAsync(string to, string templateCode, Dictionary<string, string> data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía un SMS usando una plantilla
    /// </summary>
    Task<bool> SendSmsAsync(string phone, string templateCode, Dictionary<string, string> data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renderiza una plantilla con datos
    /// </summary>
    Task<(string subject, string body)> RenderTemplateAsync(string templateCode, Dictionary<string, string> data, CancellationToken cancellationToken = default);
}
