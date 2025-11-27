using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Appointments;
using ElectroHuila.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio para gestión de citas (Appointments).
/// Proporciona métodos para consultar, crear, actualizar y eliminar citas.
/// Todas las consultas incluyen las relaciones con Cliente, Sucursal y Tipo de Cita.
/// </summary>
public class AppointmentRepository : IAppointmentRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AppointmentRepository> _logger;

    /// <summary>
    /// Constructor del repositorio de citas.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    /// <param name="logger">Logger para diagnóstico.</param>
    public AppointmentRepository(ApplicationDbContext context, ILogger<AppointmentRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Validar que la conexión esté configurada
        if (_context.Database.GetDbConnection() == null)
        {
            _logger.LogError("Database connection is not configured properly in AppointmentRepository");
            throw new InvalidOperationException("Database connection is not configured properly");
        }
    }

    /// <summary>
    /// Obtiene una cita por su ID, incluyendo relaciones con Client, Branch y AppointmentType.
    /// </summary>
    /// <param name="id">ID de la cita.</param>
    /// <returns>Cita encontrada con sus relaciones, o null si no existe o está inactiva.</returns>
    public async Task<Appointment?> GetByIdAsync(int id)
    {
        try
        {
            if (_context == null)
            {
                _logger.LogError("DbContext is null in GetByIdAsync");
                throw new InvalidOperationException("Database context is not initialized");
            }

            // NO usar .Include() - causa NullReferenceException en Oracle con tablas relacionadas nullable
            var appointment = await _context.Appointments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

            if (appointment == null)
                return null;

            // Cargar relaciones manualmente con queries separadas
            appointment.Client = await _context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == appointment.ClientId);

            appointment.Branch = await _context.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == appointment.BranchId);

            appointment.AppointmentType = await _context.AppointmentTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(at => at.Id == appointment.AppointmentTypeId);

            return appointment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointment with ID {AppointmentId}. Exception: {ExceptionType}, Message: {Message}",
                id, ex.GetType().Name, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las citas activas con sus relaciones.
    /// </summary>
    /// <returns>Colección de todas las citas activas.</returns>
    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Client)
            .Include(a => a.Branch)
            .Include(a => a.AppointmentType)
            .Where(a => a.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todas las citas incluyendo inactivas con sus relaciones.
    /// </summary>
    /// <returns>Colección de todas las citas (activas e inactivas).</returns>
    public async Task<IEnumerable<Appointment>> GetAllIncludingInactiveAsync()
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Client)
            .Include(a => a.Branch)
            .Include(a => a.AppointmentType)
            .Include(a => a.Status)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todas las citas de un cliente específico.
    /// </summary>
    /// <param name="clientId">ID del cliente.</param>
    /// <returns>Colección de citas del cliente.</returns>
    public async Task<IEnumerable<Appointment>> GetByClientIdAsync(int clientId)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Client)
            .Include(a => a.Branch)
            .Include(a => a.AppointmentType)
            .Include(a => a.Status)
            .Where(a => a.ClientId == clientId && a.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todas las citas de una sucursal específica.
    /// </summary>
    /// <param name="branchId">ID de la sucursal.</param>
    /// <returns>Colección de citas de la sucursal.</returns>
    public async Task<IEnumerable<Appointment>> GetByBranchIdAsync(int branchId)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Client)
            .Include(a => a.Branch)
            .Include(a => a.AppointmentType)
            .Where(a => a.BranchId == branchId && a.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Agrega una nueva cita a la base de datos.
    /// </summary>
    /// <param name="appointment">Cita a agregar.</param>
    /// <returns>Cita agregada con su ID asignado.</returns>
    public async Task<Appointment> AddAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    /// <summary>
    /// Actualiza una cita existente.
    /// </summary>
    /// <param name="appointment">Cita con los datos actualizados.</param>
    public async Task UpdateAsync(Appointment appointment)
    {
        // Detach any existing tracked entity with the same ID to avoid conflicts
        var trackedEntity = _context.ChangeTracker.Entries<Appointment>()
            .FirstOrDefault(e => e.Entity.Id == appointment.Id);

        if (trackedEntity != null)
        {
            trackedEntity.State = EntityState.Detached;
        }

        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Elimina lógicamente una cita (soft delete) marcándola como inactiva.
    /// </summary>
    /// <param name="id">ID de la cita a eliminar.</param>
    public async Task DeleteAsync(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment != null)
        {
            appointment.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Verifica si existe una cita activa con el ID especificado.
    /// </summary>
    /// <param name="id">ID de la cita a verificar.</param>
    /// <returns>True si existe y está activa, False si no existe o está inactiva.</returns>
    public async Task<bool> ExistsAsync(int id)
    {
        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await _context.Appointments.CountAsync(a => a.Id == id && a.IsActive) > 0;
    }

    /// <summary>
    /// Obtiene todas las citas pendientes o no asistidas de un cliente por número de documento.
    /// Considera como pendientes/no asistidas: PENDING (1), CONFIRMED (2), NO_SHOW (3).
    /// No incluye: COMPLETED (4), CANCELLED (5).
    /// </summary>
    /// <param name="documentNumber">Número de documento del cliente.</param>
    /// <returns>Colección de citas pendientes o no asistidas.</returns>
    public async Task<IEnumerable<Appointment>> GetPendingOrNoShowAppointmentsByDocumentNumberAsync(string documentNumber)
    {
        // StatusIds: 1=PENDING, 2=CONFIRMED, 3=NO_SHOW, 4=COMPLETED, 5=CANCELLED
        var pendingStatuses = new[] { 1, 2, 3 };

        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Client)
            .Include(a => a.Status)
            .Where(a => a.Client.DocumentNumber == documentNumber
                     && a.IsActive
                     && pendingStatuses.Contains(a.StatusId))
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si un cliente tiene citas pendientes o no asistidas por número de documento.
    /// </summary>
    /// <param name="documentNumber">Número de documento del cliente.</param>
    /// <returns>True si tiene citas pendientes o no asistidas, False en caso contrario.</returns>
    public async Task<bool> HasPendingOrNoShowAppointmentsAsync(string documentNumber)
    {
        // StatusIds: 1=PENDING, 2=CONFIRMED, 3=NO_SHOW, 4=COMPLETED, 5=CANCELLED
        var pendingStatuses = new[] { 1, 2, 3 };

        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await _context.Appointments
            .Include(a => a.Client)
            .CountAsync(a => a.Client.DocumentNumber == documentNumber
                        && a.IsActive
                        && pendingStatuses.Contains(a.StatusId)) > 0;
    }

    /// <summary>
    /// Obtiene citas con datos completos (JOINs) filtradas por tipos de cita
    /// </summary>
    /// <param name="appointmentTypeIds">IDs de tipos de cita para filtrar</param>
    /// <returns>Lista de citas con datos relacionados cargados</returns>
    public async Task<IEnumerable<Appointment>> GetAppointmentsWithDetailsAsync(IEnumerable<int> appointmentTypeIds)
    {
        var query = _context.Appointments
            .AsNoTracking()
            .Include(a => a.Client)
            .Include(a => a.Branch)
            .Include(a => a.AppointmentType)
            .Include(a => a.Status)
            .Where(a => a.IsActive && appointmentTypeIds.Contains(a.AppointmentTypeId))
            .OrderByDescending(a => a.AppointmentDate)
            .ThenByDescending(a => a.CreatedAt);

        return await query.ToListAsync();
    }
}