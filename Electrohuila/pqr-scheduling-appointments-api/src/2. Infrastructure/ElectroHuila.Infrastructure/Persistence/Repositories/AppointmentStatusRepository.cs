using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Catalogs;
using ElectroHuila.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de estados de citas
/// </summary>
public class AppointmentStatusRepository : BaseRepository<AppointmentStatus>, IAppointmentStatusRepository
{
    public AppointmentStatusRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<AppointmentStatus?> GetByCodeAsync(string code)
    {
        return await _context.Set<AppointmentStatus>()
            .FirstOrDefaultAsync(s => s.Code == code.ToUpper());
    }

    public async Task<IEnumerable<AppointmentStatus>> GetAllActiveOrderedAsync()
    {
        return await _context.Set<AppointmentStatus>()
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<AppointmentStatus>> GetCancellableStatusesAsync()
    {
        return await _context.Set<AppointmentStatus>()
            .Where(s => s.IsActive && s.AllowCancellation)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<AppointmentStatus>> GetFinalStatusesAsync()
    {
        return await _context.Set<AppointmentStatus>()
            .Where(s => s.IsActive && s.IsFinalState)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
    }
}
