using ElectroHuila.Domain.Entities.Appointments;
using ElectroHuila.Domain.Entities.Common;
using ElectroHuila.Domain.Entities.Security;

namespace ElectroHuila.Domain.Entities.Notifications;

/// <summary>
/// Representa una notificación enviada a un usuario del sistema.
/// Soporta múltiples tipos de notificación: EMAIL, SMS, WHATSAPP, IN_APP.
/// </summary>
public class Notification : BaseEntity
{
    /// <summary>
    /// Constructor privado para Entity Framework.
    /// </summary>
    private Notification()
    {
    }

    /// <summary>
    /// Identificador del usuario (admin) al que se envía la notificación.
    /// Nullable porque una notificación puede ser para un USER o un CLIENT, pero no ambos.
    /// </summary>
    public int? UserId { get; private set; }

    /// <summary>
    /// Identificador del cliente al que se envía la notificación.
    /// Nullable porque una notificación puede ser para un USER o un CLIENT, pero no ambos.
    /// </summary>
    public int? ClientId { get; private set; }

    /// <summary>
    /// Identificador de la cita asociada (opcional).
    /// Null si la notificación no está relacionada con una cita específica.
    /// </summary>
    public int? AppointmentId { get; private set; }

    /// <summary>
    /// Tipo de notificación: EMAIL, SMS, WHATSAPP, IN_APP.
    /// </summary>
    public string Type { get; private set; } = string.Empty;

    /// <summary>
    /// Título o asunto de la notificación.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Contenido del mensaje de la notificación.
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Estado actual de la notificación: PENDING, SENT, FAILED.
    /// </summary>
    public string Status { get; private set; } = "PENDING";

    /// <summary>
    /// Fecha y hora en que la notificación fue enviada exitosamente.
    /// Null si aún no se ha enviado.
    /// </summary>
    public DateTime? SentAt { get; private set; }

    /// <summary>
    /// Fecha y hora en que el usuario leyó la notificación (solo para IN_APP).
    /// Null si no ha sido leída.
    /// </summary>
    public DateTime? ReadAt { get; private set; }

    /// <summary>
    /// Indica si la notificación ha sido leída por el usuario (solo para IN_APP).
    /// </summary>
    public bool IsRead { get; private set; }

    /// <summary>
    /// Mensaje de error si el envío falló.
    /// Null si no hubo errores.
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Metadata adicional en formato JSON para información complementaria.
    /// Puede contener datos como IDs de referencia, parámetros de personalización, etc.
    /// </summary>
    public string? Metadata { get; private set; }

    /// <summary>
    /// Navegación al usuario (admin) destinatario de la notificación.
    /// Null si la notificación es para un cliente.
    /// </summary>
    public virtual User? User { get; set; }

    /// <summary>
    /// Navegación al cliente destinatario de la notificación.
    /// Null si la notificación es para un usuario (admin).
    /// </summary>
    public virtual ElectroHuila.Domain.Entities.Clients.Client? Client { get; set; }

    /// <summary>
    /// Navegación a la cita asociada (si existe).
    /// </summary>
    public virtual Appointment? Appointment { get; set; }

    /// <summary>
    /// Factory method para crear una nueva notificación.
    /// </summary>
    /// <param name="type">Tipo de notificación (EMAIL, SMS, WHATSAPP, IN_APP).</param>
    /// <param name="title">Título de la notificación.</param>
    /// <param name="message">Contenido del mensaje.</param>
    /// <param name="userId">ID del usuario (admin) destinatario. Opcional si se proporciona clientId.</param>
    /// <param name="clientId">ID del cliente destinatario. Opcional si se proporciona userId.</param>
    /// <param name="appointmentId">ID de la cita asociada (opcional).</param>
    /// <param name="metadata">Metadata adicional en formato JSON (opcional).</param>
    /// <returns>Nueva instancia de Notification.</returns>
    public static Notification Create(
        string type,
        string title,
        string message,
        int? userId = null,
        int? clientId = null,
        int? appointmentId = null,
        string? metadata = null)
    {
        // Validar que se proporcione userId O clientId, pero no ambos ni ninguno
        if (!userId.HasValue && !clientId.HasValue)
            throw new ArgumentException("Debe proporcionar userId o clientId.");

        if (userId.HasValue && clientId.HasValue)
            throw new ArgumentException("No puede proporcionar userId y clientId al mismo tiempo. Debe ser uno u otro.");

        if (userId.HasValue && userId.Value <= 0)
            throw new ArgumentException("UserId debe ser mayor que cero.", nameof(userId));

        if (clientId.HasValue && clientId.Value <= 0)
            throw new ArgumentException("ClientId debe ser mayor que cero.", nameof(clientId));

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type no puede estar vacío.", nameof(type));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title no puede estar vacío.", nameof(title));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message no puede estar vacío.", nameof(message));

        var validTypes = new[] { "EMAIL", "SMS", "WHATSAPP", "IN_APP" };
        if (!validTypes.Contains(type.ToUpperInvariant()))
            throw new ArgumentException($"Type debe ser uno de: {string.Join(", ", validTypes)}.", nameof(type));

        return new Notification
        {
            UserId = userId,
            ClientId = clientId,
            Type = type.ToUpperInvariant(),
            Title = title,
            Message = message,
            AppointmentId = appointmentId,
            Metadata = metadata,
            Status = "PENDING",
            IsRead = false,
            IsActive = true
        };
    }

    /// <summary>
    /// Marca la notificación como enviada exitosamente.
    /// </summary>
    public void MarkAsSent()
    {
        Status = "SENT";
        SentAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    /// <summary>
    /// Marca la notificación como fallida con un mensaje de error.
    /// </summary>
    /// <param name="errorMessage">Descripción del error ocurrido.</param>
    public void MarkAsFailed(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("ErrorMessage no puede estar vacío.", nameof(errorMessage));

        Status = "FAILED";
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Marca la notificación como leída (solo para notificaciones IN_APP).
    /// </summary>
    public void MarkAsRead()
    {
        if (IsRead)
            return;

        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Actualiza la metadata de la notificación.
    /// </summary>
    /// <param name="metadata">Nueva metadata en formato JSON.</param>
    public void UpdateMetadata(string? metadata)
    {
        Metadata = metadata;
    }
}
