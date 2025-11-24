using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio para plantillas de notificación
/// </summary>
public class NotificationTemplateRepository : BaseRepository<NotificationTemplate>, INotificationTemplateRepository
{
    public NotificationTemplateRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene una plantilla por su código
    /// </summary>
    public async Task<NotificationTemplate?> GetByCodeAsync(string templateCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.TemplateCode == templateCode)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene todas las plantillas de un tipo específico (EMAIL, SMS, PUSH)
    /// </summary>
    public async Task<IEnumerable<NotificationTemplate>> GetByTypeAsync(string templateType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.TemplateType == templateType && t.IsActive)
            .OrderBy(t => t.TemplateName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si existe una plantilla con el código especificado
    /// </summary>
    public async Task<bool> ExistsByCodeAsync(string templateCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(t => t.TemplateCode == templateCode, cancellationToken);
    }

    /// <summary>
    /// Obtiene todas las plantillas activas
    /// </summary>
    public async Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.IsActive)
            .OrderBy(t => t.TemplateType)
            .ThenBy(t => t.TemplateName)
            .ToListAsync(cancellationToken);
    }
}
