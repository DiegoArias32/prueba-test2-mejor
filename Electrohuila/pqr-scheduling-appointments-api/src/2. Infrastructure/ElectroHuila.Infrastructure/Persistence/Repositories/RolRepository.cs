using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio para la gestión de roles en el sistema de seguridad.
/// Proporciona operaciones de acceso a datos para entidades Rol,
/// incluyendo consultas con carga eager de permisos y formularios relacionados.
/// </summary>
/// <remarks>
/// Este repositorio es fundamental en el sistema de autorización basado en roles (RBAC).
/// Características principales:
/// - Carga eager automática de RolFormPermis con Form y Permission
/// - Filtrado automático por roles activos (IsActive)
/// - Soporte para soft delete manteniendo integridad referencial
/// - Consultas optimizadas para el sistema de permisos
/// 
/// ESTRATEGIA DE CARGA:
/// La mayoría de métodos incluyen automáticamente las relaciones
/// RolFormPermis -> Form y RolFormPermis -> Permission para evitar
/// consultas N+1 y proporcionar datos completos del rol.
/// 
/// RENDIMIENTO:
/// Las consultas con Include pueden ser costosas para roles con muchos permisos.
/// Considerar métodos sin Include para casos que no requieran permisos.
/// </remarks>
public class RolRepository : BaseRepository<Rol>, IRolRepository
{
    /// <summary>
    /// Inicializa una nueva instancia del repositorio de roles.
    /// </summary>
    /// <param name="context">El contexto de la base de datos de la aplicación.</param>
    public RolRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Busca un rol por su código único.
    /// </summary>
    /// <param name="code">El código único del rol a buscar.</param>
    /// <returns>
    /// El rol encontrado con todos sus permisos y formularios cargados,
    /// o null si no existe un rol activo con ese código.
    /// </returns>
    /// <remarks>
    /// CARGA EAGER INCLUIDA:
    /// - RolFormPermis: Relaciones de permisos del rol
    /// - RolFormPermis.Form: Formularios asociados a cada permiso
    /// - RolFormPermis.Permission: Permisos específicos (CRUD flags)
    /// 
    /// FILTROS APLICADOS:
    /// - Solo roles activos (IsActive = true)
    /// - Búsqueda exacta por código
    /// 
    /// RENDIMIENTO: Consulta compleja que puede ser costosa.
    /// Usar GetByCodeLightAsync() si no se requieren permisos.
    /// </remarks>
    public async Task<Rol?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Include(r => r.RolFormPermis)
                .ThenInclude(rfp => rfp.Form)
            .Include(r => r.RolFormPermis)
                .ThenInclude(rfp => rfp.Permission)
            .FirstOrDefaultAsync(r => r.Code == code && r.IsActive);
    }

    /// <summary>
    /// Busca un rol por su nombre.
    /// </summary>
    /// <param name="name">El nombre del rol a buscar.</param>
    /// <returns>
    /// El rol encontrado con todos sus permisos y formularios cargados,
    /// o null si no existe un rol activo con ese nombre.
    /// </returns>
    /// <remarks>
    /// CARGA EAGER INCLUIDA:
    /// - RolFormPermis: Relaciones de permisos del rol
    /// - RolFormPermis.Form: Formularios asociados a cada permiso
    /// - RolFormPermis.Permission: Permisos específicos (CRUD flags)
    /// 
    /// FILTROS APLICADOS:
    /// - Solo roles activos (IsActive = true)
    /// - Búsqueda exacta por nombre (case-sensitive)
    /// 
    /// CONSIDERACIÓN: La búsqueda por nombre puede ser menos eficiente
    /// que por código. Considerar agregar índice en la columna Name.
    /// </remarks>
    public async Task<Rol?> GetByNameAsync(string name)
    {
        return await _dbSet
            .Include(r => r.RolFormPermis)
                .ThenInclude(rfp => rfp.Form)
            .Include(r => r.RolFormPermis)
                .ThenInclude(rfp => rfp.Permission)
            .FirstOrDefaultAsync(r => r.Name == name && r.IsActive);
    }

    /// <summary>
    /// Obtiene todos los roles activos del sistema.
    /// </summary>
    /// <returns>
    /// Una colección de roles activos ordenados alfabéticamente por nombre.
    /// </returns>
    /// <remarks>
    /// OPTIMIZACIÓN: Este método NO incluye permisos relacionados
    /// para mejorar el rendimiento en listados simples.
    ///
    /// FILTROS APLICADOS:
    /// - Solo roles activos (IsActive = true)
    /// - Ordenamiento alfabético por nombre
    ///
    /// CASOS DE USO:
    /// - Listados de roles para interfaces de usuario
    /// - Dropdowns de selección de roles
    /// - Reportes básicos de roles
    ///
    /// Para roles con permisos, usar GetByCodeAsync() o GetByNameAsync().
    /// </remarks>
    public async Task<IEnumerable<Rol>> GetActiveAsync()
    {
        return await _dbSet
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los roles del sistema incluyendo inactivos.
    /// </summary>
    /// <returns>
    /// Una colección de todos los roles (activos e inactivos) ordenados alfabéticamente por nombre.
    /// </returns>
    /// <remarks>
    /// OPTIMIZACIÓN: Este método NO incluye permisos relacionados
    /// para mejorar el rendimiento en listados simples.
    ///
    /// FILTROS APLICADOS:
    /// - NO filtra por IsActive, retorna todos los registros
    /// - Ordenamiento alfabético por nombre
    ///
    /// CASOS DE USO:
    /// - Administración de roles (incluyendo eliminados)
    /// - Auditoría y reportes completos
    /// - Restauración de roles eliminados
    /// </remarks>
    public async Task<IEnumerable<Rol>> GetAllIncludingInactiveAsync()
    {
        return await _dbSet
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los roles asignados a un usuario específico.
    /// </summary>
    /// <param name="userId">El identificador del usuario.</param>
    /// <returns>
    /// Una colección de roles activos asignados al usuario especificado.
    /// </returns>
    /// <remarks>
    /// RELACIÓN UTILIZADA:
    /// Consulta la tabla de unión RolUsers para encontrar roles asignados.
    /// 
    /// CARGA EAGER INCLUIDA:
    /// - RolUsers: Relaciones usuario-rol para filtrado
    /// 
    /// FILTROS APLICADOS:
    /// - Solo roles activos (IsActive = true)
    /// - Solo roles asignados al usuario específico
    /// 
    /// CASOS DE USO:
    /// - Determinar permisos de usuario en tiempo de autenticación
    /// - Validación de autorización
    /// - Interfaces de gestión de usuarios
    /// 
    /// RENDIMIENTO: Eficiente para consultas por usuario individual.
    /// </remarks>
    public async Task<IEnumerable<Rol>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(r => r.RolUsers)
            .Where(r => r.RolUsers.Any(ru => ru.UserId == userId) && r.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si existe un rol con el código especificado.
    /// </summary>
    /// <param name="code">El código del rol a verificar.</param>
    /// <returns>
    /// true si existe un rol con ese código; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// IMPORTANTE: No filtra por IsActive, incluye roles eliminados con soft delete.
    /// 
    /// CASOS DE USO:
    /// - Validación de unicidad antes de crear nuevos roles
    /// - Verificación de códigos para restaurar roles eliminados
    /// - Auditoría de códigos existentes
    /// 
    /// OPTIMIZACIÓN: Consulta muy eficiente usando AnyAsync()
    /// sin cargar datos innecesarios.
    /// 
    /// Para verificar solo roles activos, combinar con:
    /// await _dbSet.CountAsync(r => r.Code == code && r.IsActive) > 0
    /// </remarks>
    public async Task<bool> ExistsByCodeAsync(string code)
    {
        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await _dbSet.CountAsync(r => r.Code == code) > 0;
    }

    /// <summary>
    /// Verifica si existe un rol con el nombre especificado.
    /// </summary>
    /// <param name="name">El nombre del rol a verificar.</param>
    /// <returns>
    /// true si existe un rol con ese nombre; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// IMPORTANTE: No filtra por IsActive, incluye roles eliminados con soft delete.
    /// 
    /// CASOS DE USO:
    /// - Validación de unicidad antes de crear nuevos roles
    /// - Verificación de nombres para evitar duplicados
    /// - Validación en formularios de creación/edición
    /// 
    /// CONSIDERACIÓN: La búsqueda por nombre es case-sensitive.
    /// Para búsquedas case-insensitive, usar:
    /// await _dbSet.AnyAsync(r => r.Name.ToLower() == name.ToLower())
    /// 
    /// OPTIMIZACIÓN: Consulta eficiente sin carga de datos.
    /// </remarks>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await _dbSet.CountAsync(r => r.Name == name) > 0;
    }

    /// <summary>
    /// Realiza un soft delete de un rol específico.
    /// </summary>
    /// <param name="id">El identificador del rol a eliminar.</param>
    /// <remarks>
    /// IMPLEMENTACIÓN DE SOFT DELETE:
    /// Marca IsActive = false en lugar de eliminar físicamente el registro.
    /// 
    /// VENTAJAS DEL SOFT DELETE:
    /// - Preserva integridad referencial con usuarios y permisos
    /// - Permite auditoría de roles eliminados
    /// - Posibilita restauración posterior
    /// - Mantiene historial completo del sistema
    /// 
    /// IMPORTANTE: No llama a SaveChangesAsync() automáticamente.
    /// El contexto debe guardarse externamente para persistir cambios.
    /// 
    /// COMPORTAMIENTO: Silencioso si el rol no existe (no lanza excepción).
    /// 
    /// Para eliminación con persistencia automática, usar DeleteLogicalAsync().
    /// </remarks>
    public async Task DeleteAsync(int id)
    {
        var rol = await GetByIdAsync(id);
        if (rol != null)
        {
            rol.IsActive = false;
        }
    }

    /// <summary>
    /// Realiza un soft delete de un rol específico con persistencia automática.
    /// </summary>
    /// <param name="id">El identificador del rol a eliminar.</param>
    /// <remarks>
    /// DIFERENCIA CON DeleteAsync():
    /// Este método llama automáticamente a SaveChangesAsync() para persistir cambios.
    /// 
    /// IMPLEMENTACIÓN COMPLETA DE SOFT DELETE:
    /// 1. Busca el rol por ID
    /// 2. Marca IsActive = false
    /// 3. Persiste cambios en la base de datos
    /// 
    /// CASOS DE USO:
    /// - Eliminación directa desde controllers
    /// - Operaciones que requieren confirmación inmediata
    /// - APIs que necesitan respuesta de éxito/fallo
    /// 
    /// COMPORTAMIENTO: Silencioso si el rol no existe.
    /// 
    /// TRANSACCIONES: Cada llamada es una transacción independiente.
    /// Para operaciones en lote, usar DeleteAsync() + SaveChangesAsync() manual.
    /// </remarks>
    public async Task DeleteLogicalAsync(int id)
    {
        var rol = await GetByIdAsync(id);
        if (rol != null)
        {
            rol.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}