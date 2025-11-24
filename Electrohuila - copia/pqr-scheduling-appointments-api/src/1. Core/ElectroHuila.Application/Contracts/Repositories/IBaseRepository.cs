using System.Linq.Expressions;
using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Interfaz base genérica para repositorios con operaciones CRUD completas
/// </summary>
/// <typeparam name="T">Tipo de entidad que hereda de BaseEntity</typeparam>
public interface IBaseRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Obtiene una entidad por su identificador
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene todas las entidades
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Busca entidades que cumplan con el predicado
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Obtiene la primera entidad que cumple el predicado o null
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Agrega una entidad de forma asíncrona
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Agrega múltiples entidades de forma asíncrona
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Actualiza una entidad de forma asíncrona
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Actualiza una entidad de forma síncrona
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Actualiza múltiples entidades
    /// </summary>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Elimina una entidad
    /// </summary>
    void Remove(T entity);

    /// <summary>
    /// Elimina múltiples entidades
    /// </summary>
    void RemoveRange(IEnumerable<T> entities);

    /// <summary>
    /// Verifica si existe una entidad con el ID especificado
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Verifica si existe alguna entidad que cumpla el predicado
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Cuenta todas las entidades
    /// </summary>
    Task<int> CountAsync();

    /// <summary>
    /// Cuenta las entidades que cumplen el predicado
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
}
