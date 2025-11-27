using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio para la gestión de horarios disponibles para citas en la base de datos.
/// Hereda de <see cref="BaseRepository{T}"/> e implementa <see cref="IAvailableTimeRepository"/>.
/// </summary>
/// <remarks>
/// Este repositorio proporciona operaciones específicas para la entidad <see cref="AvailableTime"/>,
/// incluyendo búsquedas por sucursal, tipo de cita, validaciones de disponibilidad y gestión de soft delete.
/// Maneja la lógica de horarios disponibles para el sistema de agendamiento de citas.
/// </remarks>
public class AvailableTimeRepository : BaseRepository<AvailableTime>, IAvailableTimeRepository
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AvailableTimeRepository"/>.
    /// </summary>
    /// <param name="context">Contexto de la base de datos de la aplicación.</param>
    public AvailableTimeRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene todos los horarios disponibles activos para una sucursal específica.
    /// </summary>
    /// <param name="branchId">Identificador único de la sucursal.</param>
    /// <returns>
    /// Colección de horarios disponibles ordenados por tiempo para la sucursal especificada.
    /// </returns>
    /// <remarks>
    /// Solo incluye horarios que están activos (IsActive = true) y los ordena cronológicamente.
    /// </remarks>
    public async Task<IEnumerable<AvailableTime>> GetByBranchIdAsync(int branchId)
    {
        return await _dbSet
            .Include(at => at.Branch)
            .Include(at => at.AppointmentType)
            .Where(at => at.BranchId == branchId && at.IsActive)
            .OrderBy(at => at.Time)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los horarios disponibles para un día específico de la semana.
    /// </summary>
    /// <param name="dayOfWeek">Día de la semana a consultar.</param>
    /// <returns>
    /// Colección de horarios disponibles activos con información de sucursal incluida.
    /// </returns>
    /// <remarks>
    /// <strong>Nota importante:</strong> La entidad AvailableTime no tiene propiedad DayOfWeek.
    /// Este método retorna todos los horarios activos, el filtrado por día debe realizarse
    /// a nivel de aplicación. Se recomienda revisar el modelo si se requiere filtrado por día.
    /// </remarks>
    public async Task<IEnumerable<AvailableTime>> GetByDayOfWeekAsync(DayOfWeek dayOfWeek)
    {
        // Note: AvailableTime doesn't have DayOfWeek property anymore
        // Returning all active times, filtering by day should be done at application level
        return await _dbSet
            .Where(at => at.IsActive)
            .Include(at => at.Branch)
            .OrderBy(at => at.Time)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene horarios disponibles para una sucursal específica en un día determinado.
    /// </summary>
    /// <param name="branchId">Identificador único de la sucursal.</param>
    /// <param name="dayOfWeek">Día de la semana a consultar.</param>
    /// <returns>
    /// Colección de horarios disponibles para la sucursal especificada.
    /// </returns>
    /// <remarks>
    /// <strong>Nota importante:</strong> La entidad AvailableTime no tiene propiedad DayOfWeek.
    /// Este método retorna todos los horarios de la sucursal, el filtrado por día debe realizarse
    /// a nivel de aplicación. Se recomienda revisar el modelo si se requiere filtrado por día.
    /// </remarks>
    public async Task<IEnumerable<AvailableTime>> GetByBranchAndDayAsync(int branchId, DayOfWeek dayOfWeek)
    {
        // Note: AvailableTime doesn't have DayOfWeek property anymore
        // Returning all times for branch, filtering by day should be done at application level
        return await _dbSet
            .Where(at => at.BranchId == branchId && at.IsActive)
            .OrderBy(at => at.Time)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si hay disponibilidad de horarios en una sucursal para un rango de tiempo específico.
    /// </summary>
    /// <param name="branchId">Identificador único de la sucursal.</param>
    /// <param name="dayOfWeek">Día de la semana a verificar.</param>
    /// <param name="startTime">Hora de inicio del rango a verificar.</param>
    /// <param name="endTime">Hora de fin del rango a verificar.</param>
    /// <returns>
    /// true si hay horarios disponibles en la sucursal; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// <strong>Limitación actual:</strong> La entidad AvailableTime solo tiene propiedad Time (string).
    /// Este método se ha simplificado para verificar si existen horarios activos en la sucursal.
    /// Para validaciones de rango temporal específicas, se requiere lógica adicional a nivel de aplicación.
    /// </remarks>
    public async Task<bool> IsTimeAvailableAsync(int branchId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
    {
        // Note: AvailableTime only has Time (string) property
        // Simplified to check if any time slots exist for the branch
        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await _dbSet.CountAsync(at =>
            at.BranchId == branchId &&
            at.IsActive) > 0;
    }

    /// <summary>
    /// Obtiene todos los horarios disponibles activos del sistema.
    /// </summary>
    /// <returns>
    /// Colección de todos los horarios disponibles activos ordenados por sucursal y tiempo.
    /// </returns>
    /// <remarks>
    /// Incluye información de la sucursal relacionada mediante Include y ordena primero por sucursal
    /// y luego por tiempo para facilitar la presentación de datos.
    /// </remarks>
    public async Task<IEnumerable<AvailableTime>> GetActiveAsync()
    {
        return await _dbSet
            .Where(at => at.IsActive)
            .Include(at => at.Branch)
            .OrderBy(at => at.BranchId)
            .ThenBy(at => at.Time)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los horarios disponibles del sistema incluyendo inactivos.
    /// </summary>
    /// <returns>
    /// Colección de todos los horarios disponibles (activos e inactivos) ordenados por sucursal y tiempo.
    /// </returns>
    /// <remarks>
    /// Incluye información de la sucursal relacionada mediante Include.
    /// NO filtra por IsActive, retorna todos los registros.
    /// </remarks>
    public async Task<IEnumerable<AvailableTime>> GetAllIncludingInactiveAsync()
    {
        return await _dbSet
            .Include(at => at.Branch)
            .Include(at => at.AppointmentType)
            .OrderBy(at => at.BranchId)
            .ThenBy(at => at.Time)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene horarios disponibles para un tipo específico de cita.
    /// </summary>
    /// <param name="appointmentTypeId">Identificador único del tipo de cita.</param>
    /// <returns>
    /// Colección de horarios disponibles para el tipo de cita especificado.
    /// </returns>
    /// <remarks>
    /// Filtra por tipo de cita y estado activo, ordenando cronológicamente los resultados.
    /// </remarks>
    public async Task<IEnumerable<AvailableTime>> GetByAppointmentTypeIdAsync(int appointmentTypeId)
    {
        return await _dbSet
            .Where(at => at.AppointmentTypeId == appointmentTypeId && at.IsActive)
            .OrderBy(at => at.Time)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene horarios disponibles con filtros opcionales por sucursal y tipo de cita.
    /// </summary>
    /// <param name="branchId">Identificador único de la sucursal.</param>
    /// <param name="appointmentTypeId">Identificador opcional del tipo de cita para filtrado adicional.</param>
    /// <returns>
    /// Colección de horarios disponibles que cumplen los criterios especificados.
    /// </returns>
    /// <remarks>
    /// Método versátil que permite filtrar por sucursal obligatoriamente y opcionalmente por tipo de cita.
    /// Si appointmentTypeId es null, retorna todos los horarios de la sucursal.
    /// </remarks>
    public async Task<IEnumerable<AvailableTime>> GetAvailableTimesAsync(int branchId, int? appointmentTypeId = null)
    {
        var query = _dbSet.Where(at => at.BranchId == branchId && at.IsActive);

        if (appointmentTypeId.HasValue)
        {
            query = query.Where(at => at.AppointmentTypeId == appointmentTypeId.Value);
        }

        return await query
            .OrderBy(at => at.Time)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si un horario específico está disponible para agendamiento.
    /// </summary>
    /// <param name="branchId">Identificador único de la sucursal.</param>
    /// <param name="time">Hora específica a verificar (formato string).</param>
    /// <param name="appointmentTypeId">Identificador opcional del tipo de cita para validación adicional.</param>
    /// <returns>
    /// true si el horario está disponible; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// Verifica la existencia exacta del horario en la configuración de horarios disponibles.
    /// Útil para validar antes de crear nuevas citas en horarios específicos.
    /// </remarks>
    public async Task<bool> IsTimeSlotAvailableAsync(int branchId, string time, int? appointmentTypeId = null)
    {
        var query = _dbSet.Where(at => at.BranchId == branchId && at.IsActive);

        if (appointmentTypeId.HasValue)
        {
            query = query.Where(at => at.AppointmentTypeId == appointmentTypeId.Value);
        }

        // Check if the time exists in the available times
        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await query.CountAsync(at => at.Time == time) > 0;
    }

    /// <summary>
    /// Elimina lógicamente un horario disponible estableciendo IsActive = false.
    /// </summary>
    /// <param name="id">Identificador único del horario disponible a eliminar.</param>
    /// <returns>Task que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Implementa soft delete para preservar el historial de configuraciones de horarios.
    /// Si el horario no existe, no se realiza ninguna acción.
    /// La eliminación no afecta citas ya programadas en ese horario.
    /// </remarks>
    public async Task DeleteAsync(int id)
    {
        var availableTime = await GetByIdAsync(id);
        if (availableTime != null)
        {
            availableTime.IsActive = false;
        }
    }
}