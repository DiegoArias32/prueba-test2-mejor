using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Catalogs;
using ElectroHuila.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de estados de nuevas cuentas
/// </summary>
public class NewAccountStatusRepository : BaseRepository<NewAccountStatus>, INewAccountStatusRepository
{
    public NewAccountStatusRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<NewAccountStatus?> GetByCodeAsync(string code)
    {
        return await _context.Set<NewAccountStatus>()
            .FirstOrDefaultAsync(nas => nas.Code == code.ToUpper());
    }

    public async Task<IEnumerable<NewAccountStatus>> GetAllActiveOrderedAsync()
    {
        return await _context.Set<NewAccountStatus>()
            .Where(nas => nas.IsActive)
            .OrderBy(nas => nas.DisplayOrder)
            .ThenBy(nas => nas.Name)
            .ToListAsync();
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _context.Set<NewAccountStatus>()
            .AnyAsync(nas => nas.Code == code.ToUpper());
    }
}
