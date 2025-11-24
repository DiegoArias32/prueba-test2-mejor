using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Catalogs;
using ElectroHuila.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de tipos de proyecto
/// </summary>
public class ProjectTypeRepository : BaseRepository<ProjectType>, IProjectTypeRepository
{
    public ProjectTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ProjectType?> GetByCodeAsync(string code)
    {
        return await _context.Set<ProjectType>()
            .FirstOrDefaultAsync(pt => pt.Code == code.ToUpper());
    }

    public async Task<IEnumerable<ProjectType>> GetAllActiveOrderedAsync()
    {
        return await _context.Set<ProjectType>()
            .Where(pt => pt.IsActive)
            .OrderBy(pt => pt.DisplayOrder)
            .ThenBy(pt => pt.Name)
            .ToListAsync();
    }
}
