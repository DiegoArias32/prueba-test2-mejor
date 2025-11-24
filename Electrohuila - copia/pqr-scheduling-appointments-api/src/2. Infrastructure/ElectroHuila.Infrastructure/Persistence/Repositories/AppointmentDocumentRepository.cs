using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio para documentos adjuntos a citas
/// </summary>
public class AppointmentDocumentRepository : BaseRepository<AppointmentDocument>, IAppointmentDocumentRepository
{
    public AppointmentDocumentRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene todos los documentos de una cita específica
    /// </summary>
    public async Task<IEnumerable<AppointmentDocument>> GetByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.AppointmentId == appointmentId && d.IsActive)
            .Include(d => d.UploadedByUser)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene los documentos subidos por un usuario específico
    /// </summary>
    public async Task<IEnumerable<AppointmentDocument>> GetByUploadedByAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.UploadedBy == userId && d.IsActive)
            .Include(d => d.Appointment)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene documentos por tipo (PDF, JPG, etc.)
    /// </summary>
    public async Task<IEnumerable<AppointmentDocument>> GetByDocumentTypeAsync(string documentType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.DocumentType == documentType.ToUpperInvariant() && d.IsActive)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Elimina todos los documentos de una cita
    /// </summary>
    public async Task DeleteByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        var documents = await _dbSet
            .Where(d => d.AppointmentId == appointmentId)
            .ToListAsync(cancellationToken);

        foreach (var doc in documents)
        {
            doc.IsActive = false;
            doc.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene el tamaño total de documentos de una cita
    /// </summary>
    public async Task<long> GetTotalSizeByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.AppointmentId == appointmentId && d.IsActive && d.FileSize.HasValue)
            .SumAsync(d => d.FileSize!.Value, cancellationToken);
    }

    /// <summary>
    /// Obtiene el número de documentos de una cita
    /// </summary>
    public async Task<int> CountByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.AppointmentId == appointmentId && d.IsActive)
            .CountAsync(cancellationToken);
    }
}
