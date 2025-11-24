using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Settings;

/// <summary>
/// Configuración de tema y colores de la aplicación
/// Contiene todos los colores personalizables que se usan en el frontend
/// </summary>
public class ThemeSettings : BaseEntity
{
    /// <summary>
    /// Nombre del tema
    /// </summary>
    public string Name { get; private set; } = "Default";

    /// <summary>
    /// Descripción del tema
    /// </summary>
    public string? Description { get; private set; }

    // Colores principales
    /// <summary>
    /// Color primario principal de la aplicación (ej: #203461)
    /// </summary>
    public string ColorPrimary { get; private set; } = "#203461";

    /// <summary>
    /// Color secundario de la aplicación (ej: #1797D5)
    /// </summary>
    public string ColorSecondary { get; private set; } = "#1797D5";

    /// <summary>
    /// Color de acento (ej: #56C2E1)
    /// </summary>
    public string ColorAccent { get; private set; } = "#56C2E1";

    /// <summary>
    /// Color intermedio adicional (ej: #1A6192)
    /// </summary>
    public string ColorIntermediate { get; private set; } = "#1A6192";

    // Colores de estado
    /// <summary>
    /// Color para estados de éxito (ej: #22C55E - verde)
    /// </summary>
    public string ColorSuccess { get; private set; } = "#22C55E";

    /// <summary>
    /// Color para estados de error (ej: #EF4444 - rojo)
    /// </summary>
    public string ColorError { get; private set; } = "#EF4444";

    /// <summary>
    /// Color para advertencias (ej: #F59E0B - amarillo)
    /// </summary>
    public string ColorWarning { get; private set; } = "#F59E0B";

    /// <summary>
    /// Color para información (ej: #3B82F6 - azul)
    /// </summary>
    public string ColorInfo { get; private set; } = "#3B82F6";

    // Colores de fondo y texto
    /// <summary>
    /// Color de fondo principal
    /// </summary>
    public string BackgroundPrimary { get; private set; } = "#FFFFFF";

    /// <summary>
    /// Color de fondo secundario
    /// </summary>
    public string BackgroundSecondary { get; private set; } = "#F9FAFB";

    /// <summary>
    /// Color de texto principal
    /// </summary>
    public string TextPrimary { get; private set; } = "#111827";

    /// <summary>
    /// Color de texto secundario
    /// </summary>
    public string TextSecondary { get; private set; } = "#6B7280";

    // Configuración de scrollbar
    /// <summary>
    /// Color de inicio del gradiente del scrollbar
    /// </summary>
    public string ScrollbarGradientStart { get; private set; } = "#1797D5";

    /// <summary>
    /// Color de fin del gradiente del scrollbar
    /// </summary>
    public string ScrollbarGradientEnd { get; private set; } = "#56C2E1";

    /// <summary>
    /// Color de hover del scrollbar (inicio del gradiente)
    /// </summary>
    public string ScrollbarHoverStart { get; private set; } = "#203461";

    /// <summary>
    /// Color de hover del scrollbar (fin del gradiente)
    /// </summary>
    public string ScrollbarHoverEnd { get; private set; } = "#1797D5";

    /// <summary>
    /// Indica si este es el tema activo
    /// </summary>
    public bool IsDefaultTheme { get; private set; }

    private ThemeSettings() { } // Para EF Core

    public static ThemeSettings CreateDefault()
    {
        return new ThemeSettings
        {
            Name = "ElectroHuila Default",
            Description = "Tema por defecto de ElectroHuila",
            ColorPrimary = "#203461",
            ColorSecondary = "#1797D5",
            ColorAccent = "#56C2E1",
            ColorIntermediate = "#1A6192",
            ColorSuccess = "#22C55E",
            ColorError = "#EF4444",
            ColorWarning = "#F59E0B",
            ColorInfo = "#3B82F6",
            BackgroundPrimary = "#FFFFFF",
            BackgroundSecondary = "#F9FAFB",
            TextPrimary = "#111827",
            TextSecondary = "#6B7280",
            ScrollbarGradientStart = "#1797D5",
            ScrollbarGradientEnd = "#56C2E1",
            ScrollbarHoverStart = "#203461",
            ScrollbarHoverEnd = "#1797D5",
            IsDefaultTheme = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static ThemeSettings Create(
        string name,
        string? description = null,
        string? colorPrimary = null,
        string? colorSecondary = null,
        string? colorAccent = null,
        bool isDefaultTheme = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        var theme = new ThemeSettings
        {
            Name = name,
            Description = description,
            IsDefaultTheme = isDefaultTheme,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Apply custom colors if provided
        if (!string.IsNullOrWhiteSpace(colorPrimary))
            theme.ColorPrimary = colorPrimary;
        if (!string.IsNullOrWhiteSpace(colorSecondary))
            theme.ColorSecondary = colorSecondary;
        if (!string.IsNullOrWhiteSpace(colorAccent))
            theme.ColorAccent = colorAccent;

        return theme;
    }

    public void UpdateMainColors(string colorPrimary, string colorSecondary, string colorAccent, string? colorIntermediate = null)
    {
        if (!string.IsNullOrWhiteSpace(colorPrimary))
            ColorPrimary = colorPrimary;
        if (!string.IsNullOrWhiteSpace(colorSecondary))
            ColorSecondary = colorSecondary;
        if (!string.IsNullOrWhiteSpace(colorAccent))
            ColorAccent = colorAccent;
        if (!string.IsNullOrWhiteSpace(colorIntermediate))
            ColorIntermediate = colorIntermediate;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatusColors(string? colorSuccess = null, string? colorError = null, string? colorWarning = null, string? colorInfo = null)
    {
        if (!string.IsNullOrWhiteSpace(colorSuccess))
            ColorSuccess = colorSuccess;
        if (!string.IsNullOrWhiteSpace(colorError))
            ColorError = colorError;
        if (!string.IsNullOrWhiteSpace(colorWarning))
            ColorWarning = colorWarning;
        if (!string.IsNullOrWhiteSpace(colorInfo))
            ColorInfo = colorInfo;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBackgroundColors(string backgroundPrimary, string backgroundSecondary)
    {
        if (!string.IsNullOrWhiteSpace(backgroundPrimary))
            BackgroundPrimary = backgroundPrimary;
        if (!string.IsNullOrWhiteSpace(backgroundSecondary))
            BackgroundSecondary = backgroundSecondary;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTextColors(string textPrimary, string textSecondary)
    {
        if (!string.IsNullOrWhiteSpace(textPrimary))
            TextPrimary = textPrimary;
        if (!string.IsNullOrWhiteSpace(textSecondary))
            TextSecondary = textSecondary;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateScrollbarColors(string gradientStart, string gradientEnd, string hoverStart, string hoverEnd)
    {
        if (!string.IsNullOrWhiteSpace(gradientStart))
            ScrollbarGradientStart = gradientStart;
        if (!string.IsNullOrWhiteSpace(gradientEnd))
            ScrollbarGradientEnd = gradientEnd;
        if (!string.IsNullOrWhiteSpace(hoverStart))
            ScrollbarHoverStart = hoverStart;
        if (!string.IsNullOrWhiteSpace(hoverEnd))
            ScrollbarHoverEnd = hoverEnd;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string? description)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsDefault()
    {
        IsDefaultTheme = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAsDefault()
    {
        IsDefaultTheme = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
