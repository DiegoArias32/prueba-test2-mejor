using ElectroHuila.Infrastructure.DTOs.ExternalApis;

namespace ElectroHuila.Infrastructure.Services.ExternalApis;

/// <summary>
/// Interfaz para el servicio de integración con la API externa de WhatsApp.
/// Proporciona métodos para enviar notificaciones de citas a través de WhatsApp.
/// </summary>
public interface IWhatsAppApiService
{
    /// <summary>
    /// Envía una confirmación de cita por WhatsApp.
    /// </summary>
    /// <param name="phoneNumber">Número de teléfono del destinatario (formato internacional: +57XXXXXXXXXX)</param>
    /// <param name="data">Datos de la confirmación de cita</param>
    /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
    /// <returns>True si el envío fue exitoso, False en caso contrario</returns>
    Task<bool> SendAppointmentConfirmationAsync(
        string phoneNumber,
        AppointmentConfirmationData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía un recordatorio de cita por WhatsApp.
    /// </summary>
    /// <param name="phoneNumber">Número de teléfono del destinatario (formato internacional: +57XXXXXXXXXX)</param>
    /// <param name="data">Datos del recordatorio de cita</param>
    /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
    /// <returns>True si el envío fue exitoso, False en caso contrario</returns>
    Task<bool> SendAppointmentReminderAsync(
        string phoneNumber,
        AppointmentReminderData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía una notificación de cancelación de cita por WhatsApp.
    /// </summary>
    /// <param name="phoneNumber">Número de teléfono del destinatario (formato internacional: +57XXXXXXXXXX)</param>
    /// <param name="data">Datos de la cancelación de cita</param>
    /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
    /// <returns>True si el envío fue exitoso, False en caso contrario</returns>
    Task<bool> SendAppointmentCancellationAsync(
        string phoneNumber,
        AppointmentCancellationData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica el estado de conectividad con la API de WhatsApp.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
    /// <returns>True si la API está disponible, False en caso contrario</returns>
    Task<bool> CheckStatusAsync(CancellationToken cancellationToken = default);
}
