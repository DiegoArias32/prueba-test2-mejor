using ElectroHuila.Domain.Entities.Catalogs;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repositorio para festivos y días no laborables
/// </summary>
public interface IHolidayRepository : IBaseRepository<Holiday>
{
    /// <summary>
    /// Verifica si una fecha es festivo
    /// </summary>
    Task<bool> IsHolidayAsync(DateTime date, int? branchId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los festivos en un rango de fechas
    /// </summary>
    Task<IEnumerable<Holiday>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int? branchId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene los festivos de un año específico
    /// </summary>
    Task<IEnumerable<Holiday>> GetByYearAsync(int year, int? branchId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el festivo de una fecha específica
    /// </summary>
    Task<Holiday?> GetByDateAsync(DateTime date, int? branchId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los festivos de una fecha específica (puede haber múltiples)
    /// </summary>
    Task<IEnumerable<Holiday>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los festivos nacionales
    /// </summary>
    Task<IEnumerable<Holiday>> GetNationalHolidaysAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene los festivos locales de una sucursal
    /// </summary>
    Task<IEnumerable<Holiday>> GetBranchHolidaysAsync(int branchId, CancellationToken cancellationToken = default);
}
