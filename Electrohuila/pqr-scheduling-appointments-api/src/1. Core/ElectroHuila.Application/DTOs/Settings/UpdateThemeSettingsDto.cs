namespace ElectroHuila.Application.DTOs.Settings;

/// <summary>
/// DTO para actualizar la configuración del tema
/// </summary>
public class UpdateThemeSettingsDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    // Main colors
    public string? ColorPrimary { get; set; }
    public string? ColorSecondary { get; set; }
    public string? ColorAccent { get; set; }
    public string? ColorIntermediate { get; set; }

    // Status colors
    public string? ColorSuccess { get; set; }
    public string? ColorError { get; set; }
    public string? ColorWarning { get; set; }
    public string? ColorInfo { get; set; }

    // Background colors
    public string? BackgroundPrimary { get; set; }
    public string? BackgroundSecondary { get; set; }

    // Text colors
    public string? TextPrimary { get; set; }
    public string? TextSecondary { get; set; }

    // Scrollbar colors
    public string? ScrollbarGradientStart { get; set; }
    public string? ScrollbarGradientEnd { get; set; }
    public string? ScrollbarHoverStart { get; set; }
    public string? ScrollbarHoverEnd { get; set; }
}
