namespace ElectroHuila.Application.Common.Interfaces.Services.Common;

/// <summary>
/// Servicio para enviar correos electrónicos
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envía un correo electrónico genérico
    /// </summary>
    /// <param name="to">Dirección de correo del destinatario</param>
    /// <param name="subject">Asunto del correo</param>
    /// <param name="body">Cuerpo del mensaje</param>
    /// <param name="isHtml">Indica si el cuerpo es HTML (por defecto true)</param>
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);

    /// <summary>
    /// Envía un correo de confirmación de cita
    /// </summary>
    /// <param name="to">Dirección de correo del destinatario</param>
    /// <param name="clientName">Nombre del cliente</param>
    /// <param name="appointmentNumber">Número de cita</param>
    /// <param name="appointmentDate">Fecha de la cita</param>
    /// <param name="appointmentTime">Hora de la cita</param>
    /// <param name="branchName">Nombre de la sucursal</param>
    Task SendAppointmentConfirmationAsync(string to, string clientName, string appointmentNumber, DateTime appointmentDate, string appointmentTime, string branchName);

    /// <summary>
    /// Envía un correo de recordatorio de cita
    /// </summary>
    /// <param name="to">Dirección de correo del destinatario</param>
    /// <param name="clientName">Nombre del cliente</param>
    /// <param name="appointmentNumber">Número de cita</param>
    /// <param name="appointmentDate">Fecha de la cita</param>
    /// <param name="appointmentTime">Hora de la cita</param>
    /// <param name="branchName">Nombre de la sucursal</param>
    Task SendAppointmentReminderAsync(string to, string clientName, string appointmentNumber, DateTime appointmentDate, string appointmentTime, string branchName);

    /// <summary>
    /// Envía un correo de cancelación de cita
    /// </summary>
    /// <param name="to">Dirección de correo del destinatario</param>
    /// <param name="clientName">Nombre del cliente</param>
    /// <param name="appointmentNumber">Número de cita</param>
    /// <param name="appointmentDate">Fecha de la cita</param>
    /// <param name="appointmentTime">Hora de la cita</param>
    /// <param name="reason">Razón de la cancelación</param>
    Task SendAppointmentCancellationAsync(string to, string clientName, string appointmentNumber, DateTime appointmentDate, string appointmentTime, string reason);

    /// <summary>
    /// Envía un correo de bienvenida a un nuevo cliente
    /// </summary>
    /// <param name="to">Dirección de correo del destinatario</param>
    /// <param name="clientName">Nombre del cliente</param>
    /// <param name="clientNumber">Número de cliente generado</param>
    Task SendWelcomeEmailAsync(string to, string clientName, string clientNumber);
}