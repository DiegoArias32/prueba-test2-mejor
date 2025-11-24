using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Catalogs;
using ElectroHuila.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de tipos de propiedad
/// </summary>
public class PropertyTypeRepository : BaseRepository<PropertyType>, IPropertyTypeRepository
{
    public PropertyTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PropertyType?> GetByCodeAsync(string code)
    {
        return await _context.Set<PropertyType>()
            .FirstOrDefaultAsync(pt => pt.Code == code.ToUpper());
    }

    public async Task<IEnumerable<PropertyType>> GetAllActiveOrderedAsync()
    {
        return await _context.Set<PropertyType>()
            .Where(pt => pt.IsActive)
            .OrderBy(pt => pt.DisplayOrder)
            .ThenBy(pt => pt.Name)
            .ToListAsync();
    }
}
