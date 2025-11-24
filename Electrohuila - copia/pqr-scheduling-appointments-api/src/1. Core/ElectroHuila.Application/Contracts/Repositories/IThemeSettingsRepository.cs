using ElectroHuila.Domain.Entities.Settings;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repositorio para la configuración de temas
/// </summary>
public interface IThemeSettingsRepository : IBaseRepository<ThemeSettings>
{
    /// <summary>
    /// Obtiene el tema por defecto activo
    /// </summary>
    Task<ThemeSettings?> GetDefaultThemeAsync();

    /// <summary>
    /// Obtiene el tema activo (normalmente solo hay uno)
    /// </summary>
    Task<ThemeSettings?> GetActiveThemeAsync();

    /// <summary>
    /// Verifica si existe un tema con el nombre especificado
    /// </summary>
    Task<bool> ExistsByNameAsync(string name);
}
