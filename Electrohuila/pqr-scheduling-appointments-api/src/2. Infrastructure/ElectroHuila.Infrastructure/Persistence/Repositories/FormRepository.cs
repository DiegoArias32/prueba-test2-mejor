using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio para la gestión de formularios del sistema de seguridad en la base de datos.
/// Hereda de <see cref="BaseRepository{T}"/> e implementa <see cref="IFormRepository"/>.
/// </summary>
/// <remarks>
/// Este repositorio proporciona operaciones específicas para la entidad <see cref="Form"/>,
/// incluyendo gestión de relaciones con módulos a través de FormModules, búsquedas por
/// identificadores únicos, validaciones de existencia y filtrado por módulos asociados.
/// Maneja automáticamente las relaciones many-to-many entre formularios y módulos.
/// </remarks>
public class FormRepository : BaseRepository<Form>, IFormRepository
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="FormRepository"/>.
    /// </summary>
    /// <param name="context">Contexto de la base de datos de la aplicación.</param>
    public FormRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Busca un formulario por su nombre incluyendo sus módulos asociados.
    /// </summary>
    /// <param name="name">Nombre del formulario a buscar.</param>
    /// <returns>
    /// El formulario encontrado con sus módulos asociados si existe y está activo; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// Incluye automáticamente la relación FormModules y los módulos asociados mediante ThenInclude.
    /// Solo considera formularios activos (IsActive = true).
    /// La búsqueda es sensible a mayúsculas y minúsculas según la configuración de la base de datos.
    /// </remarks>
    public async Task<Form?> GetByNameAsync(string name)
    {
        return await _dbSet
            .Include(f => f.FormModules)
                .ThenInclude(fm => fm.Module)
            .FirstOrDefaultAsync(f => f.Name == name && f.IsActive);
    }

    /// <summary>
    /// Busca un formulario por su código único incluyendo sus módulos asociados.
    /// </summary>
    /// <param name="code">Código único del formulario a buscar.</param>
    /// <returns>
    /// El formulario encontrado con sus módulos asociados si existe y está activo; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// Incluye automáticamente la relación FormModules y los módulos asociados mediante ThenInclude.
    /// El código debe ser único en el sistema para cada formulario activo.
    /// Solo considera formularios activos (IsActive = true).
    /// </remarks>
    public async Task<Form?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Include(f => f.FormModules)
                .ThenInclude(fm => fm.Module)
            .FirstOrDefaultAsync(f => f.Code == code && f.IsActive);
    }

    /// <summary>
    /// Obtiene todos los formularios activos del sistema ordenados alfabéticamente.
    /// </summary>
    /// <returns>
    /// Colección de formularios activos ordenados por nombre.
    /// </returns>
    /// <remarks>
    /// Solo incluye formularios que tienen IsActive = true, ordenados alfabéticamente para facilitar la presentación.
    /// No incluye las relaciones con módulos para optimizar el rendimiento en consultas masivas.
    /// </remarks>
    public async Task<IEnumerable<Form>> GetActiveAsync()
    {
        return await _dbSet
            .Where(f => f.IsActive)
            .OrderBy(f => f.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los formularios activos incluyendo sus módulos asociados.
    /// </summary>
    /// <returns>
    /// Colección de formularios activos con sus módulos asociados, ordenados por nombre.
    /// </returns>
    /// <remarks>
    /// Incluye automáticamente la relación FormModules y los módulos asociados mediante ThenInclude.
    /// Útil cuando se necesita mostrar formularios con información detallada de sus módulos.
    /// Solo considera formularios activos y los ordena alfabéticamente.
    /// </remarks>
    public async Task<IEnumerable<Form>> GetWithModulesAsync()
    {
        return await _dbSet
            .Include(f => f.FormModules)
                .ThenInclude(fm => fm.Module)
            .Where(f => f.IsActive)
            .OrderBy(f => f.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si existe un formulario con el nombre especificado.
    /// </summary>
    /// <param name="name">Nombre del formulario a verificar.</param>
    /// <returns>
    /// true si existe un formulario con ese nombre; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// Útil para validaciones de unicidad antes de crear nuevos formularios.
    /// La verificación incluye tanto formularios activos como inactivos.
    /// </remarks>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbSet.AnyAsync(f => f.Name == name);
    }

    /// <summary>
    /// Obtiene todos los formularios asociados a un módulo específico.
    /// </summary>
    /// <param name="moduleId">Identificador único del módulo.</param>
    /// <returns>
    /// Colección de formularios activos que están asociados al módulo especificado.
    /// </returns>
    /// <remarks>
    /// Utiliza la relación many-to-many a través de FormModules para filtrar formularios.
    /// Incluye automáticamente los módulos asociados a cada formulario encontrado.
    /// Solo considera formularios activos y los ordena alfabéticamente.
    /// Útil para mostrar formularios disponibles dentro de un módulo específico.
    /// </remarks>
    public async Task<IEnumerable<Form>> GetByModuleIdAsync(int moduleId)
    {
        return await _dbSet
            .Include(f => f.FormModules)
                .ThenInclude(fm => fm.Module)
            .Where(f => f.FormModules.Any(fm => fm.ModuleId == moduleId) && f.IsActive)
            .OrderBy(f => f.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si existe un formulario con el código especificado.
    /// </summary>
    /// <param name="code">Código del formulario a verificar.</param>
    /// <returns>
    /// true si existe un formulario con ese código; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// Esencial para mantener la unicidad de códigos en el sistema.
    /// La verificación incluye tanto formularios activos como inactivos.
    /// Útil para validaciones antes de crear o actualizar formularios.
    /// </remarks>
    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _dbSet.AnyAsync(f => f.Code == code);
    }

    /// <summary>
    /// Elimina lógicamente un formulario estableciendo IsActive = false.
    /// </summary>
    /// <param name="id">Identificador único del formulario a eliminar.</param>
    /// <returns>Task que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Implementa soft delete para preservar el historial del formulario y sus relaciones.
    /// Las asociaciones con módulos se mantienen para referencia histórica.
    /// Si el formulario no existe, no se realiza ninguna acción.
    /// 
    /// <para><strong>Consideraciones importantes:</strong></para>
    /// - Los permisos asociados al formulario pueden verse afectados
    /// - Las relaciones con módulos se preservan para auditoría
    /// - Verificar dependencias antes de eliminar formularios críticos del sistema
    /// </remarks>
    public async Task DeleteAsync(int id)
    {
        var form = await GetByIdAsync(id);
        if (form != null)
        {
            form.IsActive = false;
        }
    }
}