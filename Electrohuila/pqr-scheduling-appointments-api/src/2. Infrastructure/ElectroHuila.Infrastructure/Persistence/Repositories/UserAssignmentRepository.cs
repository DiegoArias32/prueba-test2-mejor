using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Assignments;
using ElectroHuila.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio para gestión de asignaciones de usuarios a tipos de cita.
/// Proporciona métodos para consultar, crear y eliminar asignaciones.
/// </summary>
public class UserAssignmentRepository : IUserAssignmentRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserAssignmentRepository> _logger;

    /// <summary>
    /// Constructor del repositorio de asignaciones.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    /// <param name="logger">Logger para diagnóstico.</param>
    public UserAssignmentRepository(ApplicationDbContext context, ILogger<UserAssignmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene una asignación por su ID, incluyendo relaciones.
    /// </summary>
    public async Task<UserAppointmentTypeAssignment?> GetByIdAsync(int id)
    {
        return await _context.UserAppointmentTypeAssignments
            .Include(ua => ua.User)
            .Include(ua => ua.AppointmentType)
            .FirstOrDefaultAsync(ua => ua.Id == id && ua.IsActive);
    }

    /// <summary>
    /// Obtiene todas las asignaciones activas con sus relaciones.
    /// </summary>
    public async Task<IEnumerable<UserAppointmentTypeAssignment>> GetAllAsync()
    {
        return await _context.UserAppointmentTypeAssignments
            .Include(ua => ua.User)
            .Include(ua => ua.AppointmentType)
            .Where(ua => ua.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todas las asignaciones de un usuario específico.
    /// </summary>
    public async Task<IEnumerable<UserAppointmentTypeAssignment>> GetByUserIdAsync(int userId)
    {
        return await _context.UserAppointmentTypeAssignments
            .Include(ua => ua.User)
            .Include(ua => ua.AppointmentType)
            .Where(ua => ua.UserId == userId && ua.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todas las asignaciones para un tipo de cita específico.
    /// </summary>
    public async Task<IEnumerable<UserAppointmentTypeAssignment>> GetByAppointmentTypeIdAsync(int appointmentTypeId)
    {
        return await _context.UserAppointmentTypeAssignments
            .Include(ua => ua.User)
            .Include(ua => ua.AppointmentType)
            .Where(ua => ua.AppointmentTypeId == appointmentTypeId && ua.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene los IDs de tipos de cita asignados a un usuario.
    /// </summary>
    public async Task<List<int>> GetAssignedAppointmentTypeIdsAsync(int userId)
    {
        _logger.LogInformation("DEBUG ASSIGNMENT REPO: Getting assignments for UserId: {UserId}", userId);

        var query = _context.UserAppointmentTypeAssignments
            .Where(ua => ua.UserId == userId && ua.IsActive);

        var sql = query.ToQueryString();
        _logger.LogInformation("DEBUG ASSIGNMENT REPO: SQL: {SQL}", sql);

        var result = await query.Select(ua => ua.AppointmentTypeId).ToListAsync();

        _logger.LogInformation("DEBUG ASSIGNMENT REPO: Found {Count} assignments: [{Ids}]",
            result.Count,
            string.Join(", ", result));

        return result;
    }

    /// <summary>
    /// Verifica si existe una asignación específica activa.
    /// </summary>
    public async Task<bool> ExistsAsync(int userId, int appointmentTypeId)
    {
        return await _context.UserAppointmentTypeAssignments
            .AnyAsync(ua => ua.UserId == userId
                         && ua.AppointmentTypeId == appointmentTypeId
                         && ua.IsActive);
    }

    /// <summary>
    /// Crea una nueva asignación.
    /// </summary>
    public async Task<UserAppointmentTypeAssignment> AddAsync(UserAppointmentTypeAssignment assignment)
    {
        _context.UserAppointmentTypeAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Recargar con las relaciones
        return await GetByIdAsync(assignment.Id) ?? assignment;
    }

    /// <summary>
    /// Elimina lógicamente una asignación (soft delete).
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var assignment = await _context.UserAppointmentTypeAssignments.FindAsync(id);
        if (assignment != null)
        {
            assignment.IsActive = false;
            assignment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Elimina lógicamente todas las asignaciones de un usuario.
    /// </summary>
    public async Task DeleteByUserIdAsync(int userId)
    {
        var assignments = await _context.UserAppointmentTypeAssignments
            .Where(ua => ua.UserId == userId && ua.IsActive)
            .ToListAsync();

        foreach (var assignment in assignments)
        {
            assignment.IsActive = false;
            assignment.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Verifica si una asignación existe y está activa.
    /// </summary>
    public async Task<bool> IsActiveAsync(int id)
    {
        return await _context.UserAppointmentTypeAssignments
            .AnyAsync(ua => ua.Id == id && ua.IsActive);
    }
}
