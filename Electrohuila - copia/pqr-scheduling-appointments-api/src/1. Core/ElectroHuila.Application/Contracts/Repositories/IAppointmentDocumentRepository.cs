using ElectroHuila.Domain.Entities.Appointments;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repositorio para documentos adjuntos a citas
/// </summary>
public interface IAppointmentDocumentRepository : IBaseRepository<AppointmentDocument>
{
    /// <summary>
    /// Obtiene todos los documentos de una cita específica
    /// </summary>
    Task<IEnumerable<AppointmentDocument>> GetByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene los documentos subidos por un usuario específico
    /// </summary>
    Task<IEnumerable<AppointmentDocument>> GetByUploadedByAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene documentos por tipo (PDF, JPG, etc.)
    /// </summary>
    Task<IEnumerable<AppointmentDocument>> GetByDocumentTypeAsync(string documentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina todos los documentos de una cita
    /// </summary>
    Task DeleteByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el tamaño total de documentos de una cita
    /// </summary>
    Task<long> GetTotalSizeByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el número de documentos de una cita
    /// </summary>
    Task<int> CountByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default);
}
