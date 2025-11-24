using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio para la gestión de permisos en el sistema de seguridad.
/// Proporciona operaciones de acceso a datos para entidades Permission,
/// incluyendo consultas por nombre, acción, roles y formularios.
/// </summary>
/// <remarks>
/// Este repositorio maneja la lógica de permisos del sistema de autorización,
/// permitiendo búsquedas complejas basadas en relaciones con roles y formularios.
/// Utiliza soft delete a través del campo IsActive para mantener integridad referencial.
/// 
/// Nota: Algunas inconsistencias detectadas en los métodos que buscan por "acción"
/// ya que la entidad Permission no parece tener una propiedad Action explícita.
/// </remarks>
public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    /// <summary>
    /// Inicializa una nueva instancia del repositorio de permisos.
    /// </summary>
    /// <param name="context">El contexto de la base de datos de la aplicación.</param>
    public PermissionRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Busca un permiso por su nombre.
    /// </summary>
    /// <param name="name">El nombre del permiso a buscar.</param>
    /// <returns>
    /// El permiso encontrado o null si no existe un permiso activo con ese nombre.
    /// </returns>
    /// <remarks>
    /// Nota: Actualmente busca por Id.ToString() == name, lo cual puede no ser la
    /// implementación esperada si se busca por nombre real del permiso.
    /// </remarks>
    public async Task<Permission?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Id.ToString() == name && p.IsActive);
    }

    /// <summary>
    /// Busca un permiso por su acción.
    /// </summary>
    /// <param name="action">La acción del permiso a buscar.</param>
    /// <returns>
    /// El permiso encontrado o null si no existe un permiso activo con esa acción.
    /// </returns>
    /// <remarks>
    /// Nota: Actualmente busca por Id.ToString() == action, lo cual puede no ser correcto
    /// si Permission no tiene una propiedad Action específica.
    /// </remarks>
    public async Task<Permission?> GetByActionAsync(string action)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Id.ToString() == action && p.IsActive);
    }

    /// <summary>
    /// Obtiene todos los permisos activos del sistema.
    /// </summary>
    /// <returns>
    /// Una colección de permisos activos ordenados por Id.
    /// </returns>
    public async Task<IEnumerable<Permission>> GetActiveAsync()
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los permisos asociados a un rol específico.
    /// </summary>
    /// <param name="roleId">El identificador del rol.</param>
    /// <returns>
    /// Una colección de permisos únicos activos asociados al rol, ordenados por Id.
    /// </returns>
    /// <remarks>
    /// Utiliza la relación RolFormPermis para encontrar permisos asociados al rol.
    /// El resultado se filtra para eliminar duplicados con Distinct().
    /// </remarks>
    public async Task<IEnumerable<Permission>> GetByRoleIdAsync(int roleId)
    {
        return await _dbSet
            .Where(p => p.RolFormPermis.Any(rfp => rfp.RolId == roleId) && p.IsActive)
            .Distinct()
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si existe un permiso con el nombre especificado.
    /// </summary>
    /// <param name="name">El nombre del permiso a verificar.</param>
    /// <returns>
    /// true si existe un permiso con ese nombre; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// No filtra por IsActive, por lo que incluye permisos eliminados con soft delete.
    /// Actualmente compara Id.ToString() con el name proporcionado.
    /// </remarks>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbSet.AnyAsync(p => p.Id.ToString() == name);
    }

    /// <summary>
    /// Obtiene todos los permisos asociados a un rol específico.
    /// </summary>
    /// <param name="rolId">El identificador del rol.</param>
    /// <returns>
    /// Una colección de permisos únicos activos asociados al rol, ordenados por Id.
    /// </returns>
    /// <remarks>
    /// Método duplicado de GetByRoleIdAsync. Considera consolidar la funcionalidad
    /// para evitar código duplicado.
    /// </remarks>
    public async Task<IEnumerable<Permission>> GetByRolIdAsync(int rolId)
    {
        return await _dbSet
            .Where(p => p.RolFormPermis.Any(rfp => rfp.RolId == rolId) && p.IsActive)
            .Distinct()
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los permisos asociados a un formulario específico.
    /// </summary>
    /// <param name="formId">El identificador del formulario.</param>
    /// <returns>
    /// Una colección de permisos únicos activos asociados al formulario, ordenados por Id.
    /// </returns>
    /// <remarks>
    /// Utiliza la relación RolFormPermis para encontrar permisos asociados al formulario.
    /// El resultado se filtra para eliminar duplicados con Distinct().
    /// </remarks>
    public async Task<IEnumerable<Permission>> GetByFormIdAsync(int formId)
    {
        return await _dbSet
            .Where(p => p.RolFormPermis.Any(rfp => rfp.FormId == formId) && p.IsActive)
            .Distinct()
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si existe un permiso con la acción especificada.
    /// </summary>
    /// <param name="action">La acción del permiso a verificar.</param>
    /// <returns>
    /// Siempre true ya que verifica si existe algún permiso con Id > 0.
    /// </returns>
    /// <remarks>
    /// Implementación incorrecta: no valida la acción específica, solo verifica
    /// que existan permisos en la tabla. Requiere revisión de la lógica.
    /// </remarks>
    public async Task<bool> ExistsByActionAsync(string action)
    {
        // Permission doesn't have an Action property, check by Id instead
        return await _dbSet.AnyAsync(p => p.Id > 0);
    }

    /// <summary>
    /// Realiza un soft delete de un permiso específico.
    /// </summary>
    /// <param name="id">El identificador del permiso a eliminar.</param>
    /// <remarks>
    /// Implementa soft delete marcando IsActive como false en lugar de eliminar
    /// físicamente el registro de la base de datos. Esto preserva la integridad
    /// referencial y permite auditoría de permisos eliminados.
    /// 
    /// El método es silencioso si el permiso no existe (no lanza excepción).
    /// </remarks>
    public async Task DeleteAsync(int id)
    {
        var permission = await GetByIdAsync(id);
        if (permission != null)
        {
            permission.IsActive = false;
        }
    }
}