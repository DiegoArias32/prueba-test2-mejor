using ElectroHuila.Domain.Entities.Common;
using System.Linq.Expressions;

namespace ElectroHuila.Application.Common.Interfaces.Persistence;

/// <summary>
/// Interfaz genérica para operaciones de solo lectura en el repositorio
/// </summary>
/// <typeparam name="T">Tipo de entidad que hereda de BaseEntity</typeparam>
public interface IReadRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Obtiene una entidad por su identificador
    /// </summary>
    /// <param name="id">Identificador de la entidad</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Entidad encontrada o null si no existe</returns>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las entidades del repositorio
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Colección de todas las entidades</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca entidades que cumplan con el predicado especificado
    /// </summary>
    /// <param name="predicate">Expresión lambda para filtrar entidades</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Colección de entidades que cumplen la condición</returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la primera entidad que cumple con el predicado o null si no existe
    /// </summary>
    /// <param name="predicate">Expresión lambda para filtrar entidades</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Primera entidad encontrada o null</returns>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si existe alguna entidad que cumpla con el predicado
    /// </summary>
    /// <param name="predicate">Expresión lambda para filtrar entidades</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si existe al menos una entidad que cumple la condición</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cuenta las entidades que cumplen con el predicado
    /// </summary>
    /// <param name="predicate">Expresión lambda para filtrar entidades</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de entidades que cumplen la condición</returns>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}