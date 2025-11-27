using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio para gestión de usuarios del sistema.
/// Proporciona métodos para consultar usuarios por diferentes criterios y obtener sus permisos.
/// </summary>
public class UserRepository : BaseRepository<User>, IUserRepository
{
    /// <summary>
    /// Constructor del repositorio de usuarios.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene un usuario por ID, incluyendo sus roles.
    /// Oculta el método base para incluir RolUsers y Rol.
    /// </summary>
    /// <param name="id">ID del usuario.</param>
    /// <returns>Usuario con sus roles o null si no existe.</returns>
    public new async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(u => u.RolUsers)
                .ThenInclude(ru => ru.Rol)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Obtiene todos los usuarios con sus roles.
    /// Oculta el método base para incluir RolUsers y Rol.
    /// </summary>
    /// <returns>Lista de usuarios con sus roles.</returns>
    public new async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbSet
            .Include(u => u.RolUsers)
                .ThenInclude(ru => ru.Rol)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los usuarios incluyendo los inactivos con sus roles.
    /// </summary>
    /// <returns>Lista de todos los usuarios (activos e inactivos) con sus roles.</returns>
    public async Task<IEnumerable<User>> GetAllIncludingInactiveAsync()
    {
        return await _dbSet
            .Include(u => u.RolUsers)
                .ThenInclude(ru => ru.Rol)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un usuario por su email, incluyendo sus roles.
    /// </summary>
    /// <param name="email">Email del usuario.</param>
    /// <returns>Usuario encontrado o null si no existe o está inactivo.</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.RolUsers)
                .ThenInclude(ru => ru.Rol)
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    /// <summary>
    /// Obtiene un usuario por su username, incluyendo roles, formularios y permisos.
    /// Este método es usado principalmente para autenticación y carga todos los permisos del usuario.
    /// </summary>
    /// <param name="username">Username del usuario.</param>
    /// <returns>Usuario con toda su información de roles y permisos, o null si no existe.</returns>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .AsSplitQuery()
            .Include(u => u.RolUsers)
                .ThenInclude(ru => ru.Rol)
                    .ThenInclude(r => r.RolFormPermis)
                        .ThenInclude(rfp => rfp.Form)
            .Include(u => u.RolUsers)
                .ThenInclude(ru => ru.Rol)
                    .ThenInclude(r => r.RolFormPermis)
                        .ThenInclude(rfp => rfp.Permission)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
    }

    public async Task<User?> GetByDocumentNumberAsync(string documentNumber)
    {
        return await _dbSet
            .Include(u => u.RolUsers)
                .ThenInclude(ru => ru.Rol)
            .FirstOrDefaultAsync(u => u.Username == documentNumber && u.IsActive);
    }

    public async Task<IEnumerable<User>> GetActiveAsync()
    {
        return await _dbSet
            .Include(u => u.RolUsers)
                .ThenInclude(ru => ru.Rol)
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(int roleId)
    {
        return await _dbSet
            .Include(u => u.RolUsers)
                .ThenInclude(ru => ru.Rol)
            .Where(u => u.RolUsers.Any(ru => ru.RolId == roleId) && u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetByRoleIdAsync(int roleId)
    {
        return await _dbSet
            .Include(u => u.RolUsers)
                .ThenInclude(ru => ru.Rol)
            .Where(u => u.RolUsers.Any(ru => ru.RolId == roleId) && u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await _dbSet.CountAsync(u => u.Email == email) > 0;
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await _dbSet.CountAsync(u => u.Username == username) > 0;
    }

    public async Task<bool> ExistsByDocumentNumberAsync(string documentNumber)
    {
        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await _dbSet.CountAsync(u => u.Username == documentNumber) > 0;
    }

    public async Task DeleteAsync(int id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            user.IsActive = false;
        }
    }

    /// <summary>
    /// Obtiene todos los permisos de un usuario agrupados por formulario.
    /// </summary>
    /// <param name="userId">ID del usuario.</param>
    /// <returns>Objeto con los permisos agrupados por formulario, incluyendo CanRead, CanCreate, CanUpdate, CanDelete.</returns>
    /// <remarks>
    /// Este método:
    /// 1. Carga el usuario con todos sus roles y permisos
    /// 2. Agrupa los permisos por formulario
    /// 3. Para cada formulario, lista los permisos CRUD del usuario
    /// 4. Retorna un objeto estructurado con formId, formName, formCode y permissions
    /// </remarks>
    public async Task<object> GetUserPermissionsAsync(int userId)
    {
        var user = await _dbSet
            .AsSplitQuery()
            .Include(u => u.RolUsers)
                .ThenInclude(ru => ru.Rol)
                    .ThenInclude(r => r.RolFormPermis)
                        .ThenInclude(rfp => rfp.Form)
            .Include(u => u.RolUsers)
                .ThenInclude(ru => ru.Rol)
                    .ThenInclude(r => r.RolFormPermis)
                        .ThenInclude(rfp => rfp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

        if (user == null)
        {
            return new { permissions = new List<object>() };
        }

        var permissions = user.RolUsers
            .SelectMany(ru => ru.Rol.RolFormPermis)
            .GroupBy(rfp => rfp.Form)
            .Select(g => new
            {
                formId = g.Key.Id,
                formName = g.Key.Name,
                formCode = g.Key.Code,
                permissions = g.Select(rfp => new
                {
                    permissionId = rfp.PermissionId,
                    canRead = rfp.Permission.CanRead,
                    canCreate = rfp.Permission.CanCreate,
                    canUpdate = rfp.Permission.CanUpdate,
                    canDelete = rfp.Permission.CanDelete
                }).ToList()
            }).ToList();

        return new { forms = permissions };
    }
}