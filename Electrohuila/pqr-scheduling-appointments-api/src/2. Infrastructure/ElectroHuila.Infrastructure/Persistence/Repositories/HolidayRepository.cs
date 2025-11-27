using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Catalogs;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio para festivos y días no laborables
/// </summary>
public class HolidayRepository : BaseRepository<Holiday>, IHolidayRepository
{
    public HolidayRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Verifica si una fecha es festivo
    /// </summary>
    public async Task<bool> IsHolidayAsync(DateTime date, int? branchId = null, CancellationToken cancellationToken = default)
    {
        var dateOnly = date.Date;

        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await _dbSet
            .Where(h => h.HolidayDate.Date == dateOnly && h.IsActive)
            .Where(h => h.BranchId == null || h.BranchId == branchId)
            .CountAsync(cancellationToken) > 0;
    }

    /// <summary>
    /// Obtiene todos los festivos en un rango de fechas
    /// </summary>
    public async Task<IEnumerable<Holiday>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int? branchId = null, CancellationToken cancellationToken = default)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        var query = _dbSet
            .Where(h => h.HolidayDate.Date >= start && h.HolidayDate.Date <= end && h.IsActive);

        if (branchId.HasValue)
        {
            query = query.Where(h => h.BranchId == null || h.BranchId == branchId);
        }

        return await query
            .OrderBy(h => h.HolidayDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene los festivos de un año específico
    /// </summary>
    public async Task<IEnumerable<Holiday>> GetByYearAsync(int year, int? branchId = null, CancellationToken cancellationToken = default)
    {
        var startDate = new DateTime(year, 1, 1);
        var endDate = new DateTime(year, 12, 31);

        return await GetByDateRangeAsync(startDate, endDate, branchId, cancellationToken);
    }

    /// <summary>
    /// Obtiene el festivo de una fecha específica
    /// </summary>
    public async Task<Holiday?> GetByDateAsync(DateTime date, int? branchId = null, CancellationToken cancellationToken = default)
    {
        var dateOnly = date.Date;

        var query = _dbSet
            .Where(h => h.HolidayDate.Date == dateOnly && h.IsActive);

        if (branchId.HasValue)
        {
            query = query.Where(h => h.BranchId == null || h.BranchId == branchId);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene todos los festivos nacionales
    /// </summary>
    public async Task<IEnumerable<Holiday>> GetNationalHolidaysAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(h => h.HolidayType == "NATIONAL" && h.IsActive)
            .OrderBy(h => h.HolidayDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene los festivos locales de una sucursal
    /// </summary>
    public async Task<IEnumerable<Holiday>> GetBranchHolidaysAsync(int branchId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(h => h.BranchId == branchId && h.IsActive)
            .OrderBy(h => h.HolidayDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene todos los festivos de una fecha específica (puede haber múltiples)
    /// </summary>
    Task<IEnumerable<Holiday>> IHolidayRepository.GetByDateAsync(DateTime date, CancellationToken cancellationToken)
    {
        var dateOnly = date.Date;

        return Task.FromResult<IEnumerable<Holiday>>(_dbSet
            .Where(h => h.HolidayDate.Date == dateOnly && h.IsActive)
            .OrderBy(h => h.HolidayType)
            .ToList());
    }
}
