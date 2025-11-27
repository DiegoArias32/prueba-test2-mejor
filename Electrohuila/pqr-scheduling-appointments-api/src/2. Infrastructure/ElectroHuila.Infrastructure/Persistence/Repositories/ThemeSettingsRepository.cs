using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio para la configuración de temas
/// </summary>
public class ThemeSettingsRepository : BaseRepository<ThemeSettings>, IThemeSettingsRepository
{
    public ThemeSettingsRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene el tema por defecto activo
    /// </summary>
    public async Task<ThemeSettings?> GetDefaultThemeAsync()
    {
        return await _dbSet
            .Where(t => t.IsDefaultTheme && t.IsActive)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Obtiene el tema activo (normalmente solo hay uno)
    /// </summary>
    public async Task<ThemeSettings?> GetActiveThemeAsync()
    {
        return await _dbSet
            .Where(t => t.IsActive)
            .OrderByDescending(t => t.IsDefaultTheme)
            .ThenByDescending(t => t.UpdatedAt)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Verifica si existe un tema con el nombre especificado
    /// </summary>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await _dbSet.CountAsync(t => t.Name == name) > 0;
    }
}
