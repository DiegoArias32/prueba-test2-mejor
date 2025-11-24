using ElectroHuila.Infrastructure.DTOs.ExternalApis;

namespace ElectroHuila.Infrastructure.Services.ExternalApis;

/// <summary>
/// Interfaz para el servicio de integración con la API externa de Gmail.
/// Proporciona métodos para enviar correos electrónicos a través de Gmail API.
/// </summary>
public interface IGmailApiService
{
    /// <summary>
    /// Envía una confirmación de cita por correo electrónico.
    /// </summary>
    /// <param name="email">Dirección de correo electrónico del destinatario</param>
    /// <param name="data">Datos de la confirmación de cita</param>
    /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
    /// <returns>True si el envío fue exitoso, False en caso contrario</returns>
    Task<bool> SendAppointmentConfirmationAsync(
        string email,
        AppointmentConfirmationData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía un recordatorio de cita por correo electrónico.
    /// </summary>
    /// <param name="email">Dirección de correo electrónico del destinatario</param>
    /// <param name="data">Datos del recordatorio de cita</param>
    /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
    /// <returns>True si el envío fue exitoso, False en caso contrario</returns>
    Task<bool> SendAppointmentReminderAsync(
        string email,
        AppointmentReminderData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía una notificación de cancelación de cita por correo electrónico.
    /// </summary>
    /// <param name="email">Dirección de correo electrónico del destinatario</param>
    /// <param name="data">Datos de la cancelación de cita</param>
    /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
    /// <returns>True si el envío fue exitoso, False en caso contrario</returns>
    Task<bool> SendAppointmentCancellationAsync(
        string email,
        AppointmentCancellationData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía un correo de restablecimiento de contraseña.
    /// </summary>
    /// <param name="email">Dirección de correo electrónico del destinatario</param>
    /// <param name="data">Datos del restablecimiento de contraseña</param>
    /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
    /// <returns>True si el envío fue exitoso, False en caso contrario</returns>
    Task<bool> SendPasswordResetAsync(
        string email,
        PasswordResetData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía un correo de bienvenida a un nuevo usuario.
    /// </summary>
    /// <param name="email">Dirección de correo electrónico del destinatario</param>
    /// <param name="data">Datos del correo de bienvenida</param>
    /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
    /// <returns>True si el envío fue exitoso, False en caso contrario</returns>
    Task<bool> SendWelcomeEmailAsync(
        string email,
        WelcomeData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica el estado de conectividad con la API de Gmail.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación para operaciones asíncronas</param>
    /// <returns>True si la API está disponible, False en caso contrario</returns>
    Task<bool> CheckStatusAsync(CancellationToken cancellationToken = default);
}
