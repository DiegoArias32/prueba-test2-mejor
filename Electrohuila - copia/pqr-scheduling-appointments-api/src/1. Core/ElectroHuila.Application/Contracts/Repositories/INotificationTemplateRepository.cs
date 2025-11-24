using ElectroHuila.Domain.Entities.Notifications;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repositorio para plantillas de notificación
/// </summary>
public interface INotificationTemplateRepository : IBaseRepository<NotificationTemplate>
{
    /// <summary>
    /// Obtiene una plantilla por su código
    /// </summary>
    Task<NotificationTemplate?> GetByCodeAsync(string templateCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las plantillas de un tipo específico (EMAIL, SMS, PUSH)
    /// </summary>
    Task<IEnumerable<NotificationTemplate>> GetByTypeAsync(string templateType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si existe una plantilla con el código especificado
    /// </summary>
    Task<bool> ExistsByCodeAsync(string templateCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las plantillas activas
    /// </summary>
    Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default);
}
