using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio base genérico que implementa operaciones CRUD comunes para entidades.
/// Todos los repositorios específicos heredan de esta clase para reutilizar funcionalidad.
/// </summary>
/// <typeparam name="T">Tipo de entidad que hereda de BaseEntity.</typeparam>
public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Contexto de base de datos de la aplicación.
    /// </summary>
    protected readonly ApplicationDbContext _context;

    /// <summary>
    /// DbSet de Entity Framework para la entidad específica.
    /// </summary>
    protected readonly DbSet<T> _dbSet;

    /// <summary>
    /// Constructor que inicializa el repositorio con el contexto de base de datos.
    /// </summary>
    /// <param name="context">Contexto de base de datos de la aplicación.</param>
    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// Obtiene una entidad por su ID.
    /// </summary>
    /// <param name="id">ID de la entidad.</param>
    /// <returns>Entidad encontrada o null si no existe.</returns>
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Obtiene todas las entidades del repositorio.
    /// </summary>
    /// <returns>Colección de todas las entidades.</returns>
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// Busca entidades que cumplan con un predicado específico.
    /// </summary>
    /// <param name="predicate">Expresión lambda para filtrar entidades.</param>
    /// <returns>Colección de entidades que cumplen el predicado.</returns>
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Obtiene la primera entidad que cumple con un predicado o null.
    /// </summary>
    /// <param name="predicate">Expresión lambda para filtrar entidades.</param>
    /// <returns>Primera entidad que cumple el predicado o null.</returns>
    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    /// <summary>
    /// Agrega una nueva entidad al repositorio y persiste los cambios en la base de datos.
    /// </summary>
    /// <param name="entity">Entidad a agregar.</param>
    /// <returns>Entidad agregada con su ID asignado por la base de datos.</returns>
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Agrega múltiples entidades al repositorio y persiste los cambios en la base de datos.
    /// </summary>
    /// <param name="entities">Colección de entidades a agregar.</param>
    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualiza una entidad existente (versión asíncrona).
    /// FIXED: Added SaveChangesAsync to persist changes to database
    /// </summary>
    /// <param name="entity">Entidad a actualizar.</param>
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualiza una entidad existente (versión síncrona).
    /// </summary>
    /// <param name="entity">Entidad a actualizar.</param>
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Actualiza múltiples entidades existentes.
    /// </summary>
    /// <param name="entities">Colección de entidades a actualizar.</param>
    public void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    /// <summary>
    /// Elimina una entidad del repositorio.
    /// </summary>
    /// <param name="entity">Entidad a eliminar.</param>
    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    /// <summary>
    /// Elimina múltiples entidades del repositorio.
    /// </summary>
    /// <param name="entities">Colección de entidades a eliminar.</param>
    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    /// <summary>
    /// Verifica si existe una entidad con el ID especificado.
    /// Oracle EF Core Bug: Usar CountAsync > 0 en lugar de AnyAsync para evitar "ORA-00904: FALSE: invalid identifier"
    /// </summary>
    /// <param name="id">ID de la entidad a verificar.</param>
    /// <returns>True si existe, False si no existe.</returns>
    public async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.CountAsync(x => x.Id == id) > 0;
    }

    /// <summary>
    /// Verifica si existe alguna entidad que cumpla con el predicado.
    /// Oracle EF Core Bug: Usar CountAsync > 0 en lugar de AnyAsync para evitar "ORA-00904: FALSE: invalid identifier"
    /// </summary>
    /// <param name="predicate">Expresión lambda para filtrar entidades.</param>
    /// <returns>True si existe al menos una entidad, False si no existe ninguna.</returns>
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate) > 0;
    }

    /// <summary>
    /// Cuenta todas las entidades del repositorio.
    /// </summary>
    /// <returns>Número total de entidades.</returns>
    public async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    /// <summary>
    /// Cuenta las entidades que cumplen con un predicado específico.
    /// </summary>
    /// <param name="predicate">Expresión lambda para filtrar entidades.</param>
    /// <returns>Número de entidades que cumplen el predicado.</returns>
    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }
}