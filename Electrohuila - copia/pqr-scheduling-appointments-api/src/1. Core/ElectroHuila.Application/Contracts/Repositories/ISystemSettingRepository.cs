using ElectroHuila.Domain.Entities.Settings;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repositorio para la configuración del sistema
/// </summary>
public interface ISystemSettingRepository : IBaseRepository<SystemSetting>
{
    /// <summary>
    /// Obtiene una configuración por su clave
    /// </summary>
    Task<SystemSetting?> GetByKeyAsync(string settingKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el valor de una configuración por su clave
    /// </summary>
    Task<string?> GetValueAsync(string settingKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza el valor de una configuración existente
    /// </summary>
    Task<bool> UpdateValueAsync(string settingKey, string newValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si existe una configuración con la clave especificada
    /// </summary>
    Task<bool> ExistsByKeyAsync(string settingKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las configuraciones de un tipo específico
    /// </summary>
    Task<IEnumerable<SystemSetting>> GetByTypeAsync(string settingType, CancellationToken cancellationToken = default);
}
