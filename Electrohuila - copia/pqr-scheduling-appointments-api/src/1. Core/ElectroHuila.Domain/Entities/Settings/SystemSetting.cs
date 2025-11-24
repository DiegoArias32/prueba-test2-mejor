using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Domain.Entities.Settings;

/// <summary>
/// Configuración del sistema en runtime
/// Permite cambiar configuraciones sin recompilar la aplicación
/// </summary>
public class SystemSetting : BaseEntity
{
    /// <summary>
    /// Clave única de la configuración
    /// Ejemplo: MAX_APPOINTMENTS_PER_DAY, EMAIL_NOTIFICATIONS_ENABLED
    /// </summary>
    public string SettingKey { get; private set; } = string.Empty;

    /// <summary>
    /// Valor de la configuración
    /// </summary>
    public string? SettingValue { get; private set; }

    /// <summary>
    /// Tipo de dato del valor
    /// Valores: STRING, NUMBER, BOOLEAN, JSON
    /// </summary>
    public string SettingType { get; private set; } = "STRING";

    /// <summary>
    /// Descripción de para qué sirve esta configuración
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Indica si el valor está encriptado en la base de datos
    /// </summary>
    public bool IsEncrypted { get; private set; }

    private SystemSetting() { } // Para EF Core

    public static SystemSetting Create(
        string settingKey,
        string? settingValue,
        string settingType = "STRING",
        string? description = null,
        bool isEncrypted = false)
    {
        if (string.IsNullOrWhiteSpace(settingKey))
            throw new ArgumentException("Setting key cannot be null or empty", nameof(settingKey));

        if (!IsValidSettingType(settingType))
            throw new ArgumentException($"Invalid setting type: {settingType}", nameof(settingType));

        return new SystemSetting
        {
            SettingKey = settingKey.ToUpperInvariant(),
            SettingValue = settingValue,
            SettingType = settingType.ToUpperInvariant(),
            Description = description,
            IsEncrypted = isEncrypted,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateValue(string? value)
    {
        SettingValue = value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Encrypt()
    {
        IsEncrypted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Decrypt()
    {
        IsEncrypted = false;
        UpdatedAt = DateTime.UtcNow;
    }

    // Helper methods para obtener valores tipados
    public int GetAsInt() => int.Parse(SettingValue ?? "0");
    public bool GetAsBoolean() => bool.Parse(SettingValue ?? "false");
    public double GetAsDouble() => double.Parse(SettingValue ?? "0.0");

    private static bool IsValidSettingType(string type)
    {
        var validTypes = new[] { "STRING", "NUMBER", "BOOLEAN", "JSON" };
        return validTypes.Contains(type.ToUpperInvariant());
    }
}
