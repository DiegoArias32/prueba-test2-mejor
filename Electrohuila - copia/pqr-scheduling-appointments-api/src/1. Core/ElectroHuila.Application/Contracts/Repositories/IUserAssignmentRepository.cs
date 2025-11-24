using ElectroHuila.Domain.Entities.Assignments;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repositorio para gestionar las asignaciones de usuarios a tipos de cita
/// </summary>
public interface IUserAssignmentRepository
{
    /// <summary>
    /// Obtiene una asignación por su ID
    /// </summary>
    Task<UserAppointmentTypeAssignment?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene todas las asignaciones activas
    /// </summary>
    Task<IEnumerable<UserAppointmentTypeAssignment>> GetAllAsync();

    /// <summary>
    /// Obtiene todas las asignaciones de un usuario específico
    /// </summary>
    Task<IEnumerable<UserAppointmentTypeAssignment>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Obtiene todas las asignaciones para un tipo de cita específico
    /// </summary>
    Task<IEnumerable<UserAppointmentTypeAssignment>> GetByAppointmentTypeIdAsync(int appointmentTypeId);

    /// <summary>
    /// Obtiene los IDs de tipos de cita asignados a un usuario
    /// </summary>
    Task<List<int>> GetAssignedAppointmentTypeIdsAsync(int userId);

    /// <summary>
    /// Verifica si existe una asignación específica
    /// </summary>
    Task<bool> ExistsAsync(int userId, int appointmentTypeId);

    /// <summary>
    /// Crea una nueva asignación
    /// </summary>
    Task<UserAppointmentTypeAssignment> AddAsync(UserAppointmentTypeAssignment assignment);

    /// <summary>
    /// Elimina una asignación (soft delete)
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Elimina todas las asignaciones de un usuario
    /// </summary>
    Task DeleteByUserIdAsync(int userId);

    /// <summary>
    /// Verifica si una asignación existe y está activa
    /// </summary>
    Task<bool> IsActiveAsync(int id);
}
