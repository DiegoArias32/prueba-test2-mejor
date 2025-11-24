namespace ElectroHuila.Application.DTOs.Notifications;

/// <summary>
/// DTO para plantilla de notificación
/// </summary>
public record NotificationTemplateDto
{
    public int Id { get; init; }
    public string TemplateCode { get; init; } = string.Empty;
    public string TemplateName { get; init; } = string.Empty;
    public string? Subject { get; init; }
    public string BodyTemplate { get; init; } = string.Empty;
    public string TemplateType { get; init; } = string.Empty;
    public string? Placeholders { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO para crear una nueva plantilla de notificación
/// </summary>
public record CreateNotificationTemplateDto
{
    public string TemplateCode { get; init; } = string.Empty;
    public string TemplateName { get; init; } = string.Empty;
    public string? Subject { get; init; }
    public string BodyTemplate { get; init; } = string.Empty;
    public string TemplateType { get; init; } = "EMAIL";
    public string? Placeholders { get; init; }
}

/// <summary>
/// DTO para actualizar una plantilla de notificación
/// </summary>
public record UpdateNotificationTemplateDto
{
    public int Id { get; init; }
    public string TemplateName { get; init; } = string.Empty;
    public string? Subject { get; init; }
    public string BodyTemplate { get; init; } = string.Empty;
    public string? Placeholders { get; init; }
}

/// <summary>
/// DTO para renderizar una plantilla con datos
/// </summary>
public record RenderTemplateDto
{
    public string TemplateCode { get; init; } = string.Empty;
    public Dictionary<string, string> Data { get; init; } = new();
}

/// <summary>
/// DTO con el resultado de renderizar una plantilla
/// </summary>
public record RenderedTemplateDto
{
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string TemplateType { get; init; } = string.Empty;
}
