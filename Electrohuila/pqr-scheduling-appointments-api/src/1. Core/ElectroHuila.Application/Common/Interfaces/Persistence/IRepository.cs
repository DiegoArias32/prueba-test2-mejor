using ElectroHuila.Domain.Entities.Common;

namespace ElectroHuila.Application.Common.Interfaces.Persistence;

/// <summary>
/// Interfaz genérica para operaciones CRUD básicas en el repositorio
/// </summary>
/// <typeparam name="T">Tipo de entidad que hereda de BaseEntity</typeparam>
public interface IRepository<T> where T : BaseEntity
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
    /// Agrega una nueva entidad al repositorio
    /// </summary>
    /// <param name="entity">Entidad a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Entidad agregada con su ID generado</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    /// <param name="entity">Entidad con los cambios a aplicar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una entidad del repositorio
    /// </summary>
    /// <param name="entity">Entidad a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si existe una entidad con el ID especificado
    /// </summary>
    /// <param name="id">Identificador de la entidad</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si la entidad existe</returns>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}