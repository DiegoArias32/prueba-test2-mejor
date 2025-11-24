using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio para la gestión de módulos del sistema de seguridad en la base de datos.
/// Hereda de <see cref="BaseRepository{T}"/> e implementa <see cref="IModuleRepository"/>.
/// </summary>
/// <remarks>
/// Este repositorio proporciona operaciones específicas para la entidad <see cref="Module"/>,
/// incluyendo gestión de relaciones con formularios a través de FormModules, búsquedas por
/// identificadores únicos y validaciones de existencia. Los módulos representan agrupaciones
/// lógicas de formularios en el sistema de control de acceso y permisos.
/// </remarks>
public class ModuleRepository : BaseRepository<Module>, IModuleRepository
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ModuleRepository"/>.
    /// </summary>
    /// <param name="context">Contexto de la base de datos de la aplicación.</param>
    public ModuleRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Busca un módulo por su nombre incluyendo sus formularios asociados.
    /// </summary>
    /// <param name="name">Nombre del módulo a buscar.</param>
    /// <returns>
    /// El módulo encontrado con sus formularios asociados si existe y está activo; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// Incluye automáticamente la relación FormModules y los formularios asociados mediante ThenInclude.
    /// Solo considera módulos activos (IsActive = true).
    /// La búsqueda es sensible a mayúsculas y minúsculas según la configuración de la base de datos.
    /// Útil para mostrar un módulo con todos sus formularios disponibles.
    /// </remarks>
    public async Task<Module?> GetByNameAsync(string name)
    {
        return await _dbSet
            .Include(m => m.FormModules)
                .ThenInclude(fm => fm.Form)
            .FirstOrDefaultAsync(m => m.Name == name && m.IsActive);
    }

    /// <summary>
    /// Busca un módulo por su código único incluyendo sus formularios asociados.
    /// </summary>
    /// <param name="code">Código único del módulo a buscar.</param>
    /// <returns>
    /// El módulo encontrado con sus formularios asociados si existe y está activo; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// Incluye automáticamente la relación FormModules y los formularios asociados mediante ThenInclude.
    /// El código debe ser único en el sistema para cada módulo activo.
    /// Solo considera módulos activos (IsActive = true).
    /// Ideal para navegación y acceso directo a módulos específicos del sistema.
    /// </remarks>
    public async Task<Module?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Include(m => m.FormModules)
                .ThenInclude(fm => fm.Form)
            .FirstOrDefaultAsync(m => m.Code == code && m.IsActive);
    }

    /// <summary>
    /// Obtiene todos los módulos activos del sistema ordenados alfabéticamente.
    /// </summary>
    /// <returns>
    /// Colección de módulos activos ordenados por nombre.
    /// </returns>
    /// <remarks>
    /// Solo incluye módulos que tienen IsActive = true, ordenados alfabéticamente para facilitar la presentación.
    /// No incluye las relaciones con formularios para optimizar el rendimiento en consultas masivas.
    /// Útil para mostrar listas de módulos disponibles sin la sobrecarga de cargar todos los formularios.
    /// </remarks>
    public async Task<IEnumerable<Module>> GetActiveAsync()
    {
        return await _dbSet
            .Where(m => m.IsActive)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si existe un módulo con el nombre especificado.
    /// </summary>
    /// <param name="name">Nombre del módulo a verificar.</param>
    /// <returns>
    /// true si existe un módulo con ese nombre; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// Útil para validaciones de unicidad antes de crear nuevos módulos.
    /// La verificación incluye tanto módulos activos como inactivos para prevenir duplicados.
    /// Esencial para mantener la integridad del sistema de navegación y organización.
    /// </remarks>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbSet.AnyAsync(m => m.Name == name);
    }

    /// <summary>
    /// Verifica si existe un módulo con el código especificado.
    /// </summary>
    /// <param name="code">Código del módulo a verificar.</param>
    /// <returns>
    /// true si existe un módulo con ese código; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// Esencial para mantener la unicidad de códigos en el sistema.
    /// La verificación incluye tanto módulos activos como inactivos para prevenir duplicados.
    /// Los códigos de módulo son críticos para el routing y navegación del sistema.
    /// </remarks>
    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _dbSet.AnyAsync(m => m.Code == code);
    }

    /// <summary>
    /// Elimina lógicamente un módulo estableciendo IsActive = false.
    /// </summary>
    /// <param name="id">Identificador único del módulo a eliminar.</param>
    /// <returns>Task que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Implementa soft delete para preservar el historial del módulo y sus relaciones.
    /// Las asociaciones con formularios se mantienen para referencia histórica y auditoría.
    /// Si el módulo no existe, no se realiza ninguna acción.
    /// 
    /// <para><strong>Consideraciones críticas:</strong></para>
    /// - Los usuarios con permisos específicos del módulo pueden perder acceso
    /// - Los formularios asociados permanecen activos pero quedan huérfanos en navegación
    /// - Las rutas y menús del sistema pueden verse afectados
    /// - Verificar dependencias de navegación antes de eliminar módulos principales
    /// - Considerar reasignar formularios a otros módulos antes de la eliminación
    /// </remarks>
    public async Task DeleteAsync(int id)
    {
        var module = await GetByIdAsync(id);
        if (module != null)
        {
            module.IsActive = false;
        }
    }
}