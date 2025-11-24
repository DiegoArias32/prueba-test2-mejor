namespace ElectroHuila.Application.Common.Interfaces.Persistence;

/// <summary>
/// Interfaz para el patrón Unit of Work que coordina la escritura de cambios en la base de datos
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Guarda todos los cambios pendientes en la base de datos
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de registros afectados</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Guarda todas las entidades modificadas en la base de datos
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si se guardaron cambios exitosamente</returns>
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}