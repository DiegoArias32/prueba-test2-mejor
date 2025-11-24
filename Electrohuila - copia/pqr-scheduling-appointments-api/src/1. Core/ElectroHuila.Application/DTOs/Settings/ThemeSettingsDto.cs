namespace ElectroHuila.Application.DTOs.Settings;

/// <summary>
/// DTO para la configuración de tema de la aplicación
/// </summary>
public class ThemeSettingsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Main colors
    public string ColorPrimary { get; set; } = string.Empty;
    public string ColorSecondary { get; set; } = string.Empty;
    public string ColorAccent { get; set; } = string.Empty;
    public string ColorIntermediate { get; set; } = string.Empty;

    // Status colors
    public string ColorSuccess { get; set; } = string.Empty;
    public string ColorError { get; set; } = string.Empty;
    public string ColorWarning { get; set; } = string.Empty;
    public string ColorInfo { get; set; } = string.Empty;

    // Background colors
    public string BackgroundPrimary { get; set; } = string.Empty;
    public string BackgroundSecondary { get; set; } = string.Empty;

    // Text colors
    public string TextPrimary { get; set; } = string.Empty;
    public string TextSecondary { get; set; } = string.Empty;

    // Scrollbar colors
    public string ScrollbarGradientStart { get; set; } = string.Empty;
    public string ScrollbarGradientEnd { get; set; } = string.Empty;
    public string ScrollbarHoverStart { get; set; } = string.Empty;
    public string ScrollbarHoverEnd { get; set; } = string.Empty;

    public bool IsDefaultTheme { get; set; }
    public bool IsActive { get; set; }
}
