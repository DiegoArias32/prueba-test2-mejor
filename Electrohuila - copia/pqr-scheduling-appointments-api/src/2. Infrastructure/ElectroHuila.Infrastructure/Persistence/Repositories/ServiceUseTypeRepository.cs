using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Catalogs;
using ElectroHuila.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de tipos de uso de servicio
/// </summary>
public class ServiceUseTypeRepository : BaseRepository<ServiceUseType>, IServiceUseTypeRepository
{
    public ServiceUseTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ServiceUseType?> GetByCodeAsync(string code)
    {
        return await _context.Set<ServiceUseType>()
            .FirstOrDefaultAsync(st => st.Code == code.ToUpper());
    }

    public async Task<IEnumerable<ServiceUseType>> GetAllActiveOrderedAsync()
    {
        return await _context.Set<ServiceUseType>()
            .Where(st => st.IsActive)
            .OrderBy(st => st.DisplayOrder)
            .ThenBy(st => st.Name)
            .ToListAsync();
    }
}
