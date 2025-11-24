using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Notifications;

/// <summary>
/// Plantilla de notificación (Email, SMS, Push)
/// Permite gestionar templates desde la base de datos sin tocar código
/// </summary>
public class NotificationTemplate : BaseEntity
{
    /// <summary>
    /// Código único de la plantilla
    /// Ejemplo: APPT_CONFIRMATION, APPT_REMINDER, APPT_CANCELLATION
    /// </summary>
    public string TemplateCode { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre descriptivo de la plantilla
    /// </summary>
    public string TemplateName { get; private set; } = string.Empty;

    /// <summary>
    /// Asunto del mensaje (solo para emails)
    /// </summary>
    public string? Subject { get; private set; }

    /// <summary>
    /// Cuerpo de la plantilla con placeholders
    /// Ejemplo: "Estimado/a {{CLIENT_NAME}}, su cita..."
    /// </summary>
    public string BodyTemplate { get; private set; } = string.Empty;

    /// <summary>
    /// Tipo de notificación
    /// Valores: EMAIL, SMS, PUSH
    /// </summary>
    public string TemplateType { get; private set; } = "EMAIL";

    /// <summary>
    /// Lista de placeholders disponibles en formato JSON
    /// Ejemplo: ["CLIENT_NAME", "APPOINTMENT_DATE", "APPOINTMENT_TIME"]
    /// </summary>
    public string? Placeholders { get; private set; }

    private NotificationTemplate() { } // Para EF Core

    public static NotificationTemplate Create(
        string templateCode,
        string templateName,
        string bodyTemplate,
        string templateType = "EMAIL",
        string? subject = null,
        string? placeholders = null)
    {
        if (string.IsNullOrWhiteSpace(templateCode))
            throw new ArgumentException("Template code cannot be null or empty", nameof(templateCode));

        if (string.IsNullOrWhiteSpace(templateName))
            throw new ArgumentException("Template name cannot be null or empty", nameof(templateName));

        if (string.IsNullOrWhiteSpace(bodyTemplate))
            throw new ArgumentException("Body template cannot be null or empty", nameof(bodyTemplate));

        if (!IsValidTemplateType(templateType))
            throw new ArgumentException($"Invalid template type: {templateType}", nameof(templateType));

        return new NotificationTemplate
        {
            TemplateCode = templateCode.ToUpperInvariant(),
            TemplateName = templateName,
            Subject = subject,
            BodyTemplate = bodyTemplate,
            TemplateType = templateType.ToUpperInvariant(),
            Placeholders = placeholders,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateTemplate(string? subject, string bodyTemplate)
    {
        if (string.IsNullOrWhiteSpace(bodyTemplate))
            throw new ArgumentException("Body template cannot be null or empty", nameof(bodyTemplate));

        Subject = subject;
        BodyTemplate = bodyTemplate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string templateName, string? description = null)
    {
        if (!string.IsNullOrWhiteSpace(templateName))
            TemplateName = templateName;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePlaceholders(string? placeholders)
    {
        Placeholders = placeholders;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reemplaza los placeholders del template con valores reales
    /// </summary>
    public string RenderTemplate(Dictionary<string, string> data)
    {
        var result = BodyTemplate;

        foreach (var kvp in data)
        {
            var placeholder = $"{{{{{kvp.Key}}}}}"; // {{KEY}}
            result = result.Replace(placeholder, kvp.Value);
        }

        return result;
    }

    private static bool IsValidTemplateType(string type)
    {
        var validTypes = new[] { "EMAIL", "SMS", "PUSH" };
        return validTypes.Contains(type.ToUpperInvariant());
    }
}
