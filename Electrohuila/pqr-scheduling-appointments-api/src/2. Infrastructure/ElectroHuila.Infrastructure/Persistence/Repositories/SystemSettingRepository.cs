using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio para configuración del sistema
/// </summary>
public class SystemSettingRepository : BaseRepository<SystemSetting>, ISystemSettingRepository
{
    public SystemSettingRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene una configuración por su clave
    /// </summary>
    public async Task<SystemSetting?> GetByKeyAsync(string settingKey, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.SettingKey == settingKey)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene el valor de una configuración por su clave
    /// </summary>
    public async Task<string?> GetValueAsync(string settingKey, CancellationToken cancellationToken = default)
    {
        var setting = await GetByKeyAsync(settingKey, cancellationToken);
        return setting?.SettingValue;
    }

    /// <summary>
    /// Actualiza el valor de una configuración existente
    /// </summary>
    public async Task<bool> UpdateValueAsync(string settingKey, string newValue, CancellationToken cancellationToken = default)
    {
        var setting = await GetByKeyAsync(settingKey, cancellationToken);
        if (setting == null)
            return false;

        setting.UpdateValue(newValue);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Verifica si existe una configuración con la clave especificada
    /// </summary>
    public async Task<bool> ExistsByKeyAsync(string settingKey, CancellationToken cancellationToken = default)
    {
        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await _dbSet.CountAsync(s => s.SettingKey == settingKey, cancellationToken) > 0;
    }

    /// <summary>
    /// Obtiene todas las configuraciones de un tipo específico
    /// </summary>
    public async Task<IEnumerable<SystemSetting>> GetByTypeAsync(string settingType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.SettingType == settingType && s.IsActive)
            .OrderBy(s => s.SettingKey)
            .ToListAsync(cancellationToken);
    }
}
