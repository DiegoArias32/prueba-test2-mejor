using ElectroHuila.Domain.Entities.Appointments;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repositorio para gestionar las citas (appointments)
/// </summary>
public interface IAppointmentRepository
{
    /// <summary>
    /// Obtiene una cita por su identificador
    /// </summary>
    Task<Appointment?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene todas las citas
    /// </summary>
    Task<IEnumerable<Appointment>> GetAllAsync();

    /// <summary>
    /// Obtiene todas las citas incluyendo inactivas
    /// </summary>
    Task<IEnumerable<Appointment>> GetAllIncludingInactiveAsync();

    /// <summary>
    /// Obtiene todas las citas de un cliente específico
    /// </summary>
    Task<IEnumerable<Appointment>> GetByClientIdAsync(int clientId);

    /// <summary>
    /// Obtiene todas las citas de una sucursal específica
    /// </summary>
    Task<IEnumerable<Appointment>> GetByBranchIdAsync(int branchId);

    /// <summary>
    /// Agrega una nueva cita
    /// </summary>
    Task<Appointment> AddAsync(Appointment appointment);

    /// <summary>
    /// Actualiza una cita existente
    /// </summary>
    Task UpdateAsync(Appointment appointment);

    /// <summary>
    /// Elimina una cita por su identificador
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Verifica si existe una cita con el ID especificado
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Obtiene todas las citas pendientes o no asistidas de un cliente por número de documento
    /// </summary>
    /// <param name="documentNumber">Número de documento del cliente</param>
    /// <returns>Lista de citas pendientes o no asistidas</returns>
    Task<IEnumerable<Appointment>> GetPendingOrNoShowAppointmentsByDocumentNumberAsync(string documentNumber);

    /// <summary>
    /// Verifica si un cliente tiene citas pendientes o no asistidas por número de documento
    /// </summary>
    /// <param name="documentNumber">Número de documento del cliente</param>
    /// <returns>True si tiene citas pendientes o no asistidas, False en caso contrario</returns>
    Task<bool> HasPendingOrNoShowAppointmentsAsync(string documentNumber);

    /// <summary>
    /// Obtiene citas con datos completos (JOINs) filtradas por tipos de cita
    /// </summary>
    /// <param name="appointmentTypeIds">IDs de tipos de cita para filtrar</param>
    /// <returns>Lista de citas con datos relacionados cargados</returns>
    Task<IEnumerable<Appointment>> GetAppointmentsWithDetailsAsync(IEnumerable<int> appointmentTypeIds);
}