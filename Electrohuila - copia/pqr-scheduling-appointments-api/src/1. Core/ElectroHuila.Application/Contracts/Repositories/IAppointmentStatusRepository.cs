using ElectroHuila.Domain.Entities.Catalogs;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repositorio para los estados de citas
/// </summary>
public interface IAppointmentStatusRepository : IBaseRepository<AppointmentStatus>
{
    /// <summary>
    /// Obtiene un estado por su código
    /// </summary>
    Task<AppointmentStatus?> GetByCodeAsync(string code);

    /// <summary>
    /// Obtiene todos los estados activos ordenados
    /// </summary>
    Task<IEnumerable<AppointmentStatus>> GetAllActiveOrderedAsync();

    /// <summary>
    /// Obtiene los estados que permiten cancelación
    /// </summary>
    Task<IEnumerable<AppointmentStatus>> GetCancellableStatusesAsync();

    /// <summary>
    /// Obtiene los estados finales (no permiten transiciones)
    /// </summary>
    Task<IEnumerable<AppointmentStatus>> GetFinalStatusesAsync();
}
