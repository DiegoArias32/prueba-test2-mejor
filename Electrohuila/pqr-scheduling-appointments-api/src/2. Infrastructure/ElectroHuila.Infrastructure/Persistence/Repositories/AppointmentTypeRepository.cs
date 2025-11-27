using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio para la gestión de tipos de cita en la base de datos.
/// Hereda de <see cref="BaseRepository{T}"/> e implementa <see cref="IAppointmentTypeRepository"/>.
/// </summary>
/// <remarks>
/// Este repositorio proporciona operaciones específicas para la entidad <see cref="AppointmentType"/>,
/// incluyendo búsquedas por nombre, validaciones de existencia y gestión de soft delete.
/// Utiliza Entity Framework Core para las operaciones de base de datos.
/// </remarks>
public class AppointmentTypeRepository : BaseRepository<AppointmentType>, IAppointmentTypeRepository
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AppointmentTypeRepository"/>.
    /// </summary>
    /// <param name="context">Contexto de la base de datos de la aplicación.</param>
    public AppointmentTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Busca un tipo de cita por su nombre exacto.
    /// </summary>
    /// <param name="name">Nombre del tipo de cita a buscar.</param>
    /// <returns>
    /// El tipo de cita encontrado si existe; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// La búsqueda es sensible a mayúsculas y minúsculas según la configuración de la base de datos.
    /// </remarks>
    public async Task<AppointmentType?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(at => at.Name == name);
    }

    /// <summary>
    /// Busca un tipo de cita por su código identificador.
    /// </summary>
    /// <param name="code">Código del tipo de cita a buscar.</param>
    /// <returns>
    /// El tipo de cita encontrado si existe; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// <strong>Nota importante:</strong> La entidad AppointmentType no tiene propiedad Code,
    /// por lo que este método utiliza la propiedad Name como identificador alternativo.
    /// Se recomienda revisar el modelo de datos si se requiere un campo Code específico.
    /// </remarks>
    public async Task<AppointmentType?> GetByCodeAsync(string code)
    {
        // Note: AppointmentType doesn't have Code property
        // Using Name as identifier instead
        return await _dbSet.FirstOrDefaultAsync(at => at.Name == code);
    }

    /// <summary>
    /// Obtiene todos los tipos de cita que están activos en el sistema.
    /// </summary>
    /// <returns>
    /// Colección de tipos de cita que tienen IsActive = true.
    /// </returns>
    /// <remarks>
    /// Este método filtra automáticamente los tipos de cita que han sido eliminados lógicamente.
    /// </remarks>
    public async Task<IEnumerable<AppointmentType>> GetActiveAsync()
    {
        return await _dbSet.Where(at => at.IsActive).ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los tipos de cita incluyendo inactivos del sistema.
    /// </summary>
    /// <returns>
    /// Colección de todos los tipos de cita (activos e inactivos).
    /// </returns>
    /// <remarks>
    /// Este método NO filtra por IsActive, retorna todos los registros.
    /// </remarks>
    public async Task<IEnumerable<AppointmentType>> GetAllIncludingInactiveAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// Verifica si existe un tipo de cita con el nombre especificado.
    /// </summary>
    /// <param name="name">Nombre del tipo de cita a verificar.</param>
    /// <returns>
    /// true si existe un tipo de cita con ese nombre; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// Útil para validaciones de unicidad antes de crear o actualizar tipos de cita.
    /// La verificación incluye tanto registros activos como inactivos.
    ///
    /// NOTA: Se usa CountAsync en lugar de AnyAsync para evitar el bug del proveedor Oracle EF Core
    /// que genera literales "True/False" en lugar de 1/0 en queries CASE WHEN EXISTS.
    /// </remarks>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbSet.CountAsync(at => at.Name == name) > 0;
    }

    /// <summary>
    /// Verifica si existe un tipo de cita con el código especificado.
    /// </summary>
    /// <param name="code">Código del tipo de cita a verificar.</param>
    /// <returns>
    /// true si existe un tipo de cita con ese código; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// <strong>Nota importante:</strong> La entidad AppointmentType no tiene propiedad Code,
    /// por lo que este método utiliza la propiedad Name como identificador alternativo.
    /// Se recomienda revisar el modelo de datos si se requiere un campo Code específico.
    ///
    /// NOTA: Se usa CountAsync en lugar de AnyAsync para evitar el bug del proveedor Oracle EF Core
    /// que genera literales "True/False" en lugar de 1/0 en queries CASE WHEN EXISTS.
    /// </remarks>
    public async Task<bool> ExistsByCodeAsync(string code)
    {
        // Note: AppointmentType doesn't have Code property
        // Using Name as identifier instead
        return await _dbSet.CountAsync(at => at.Name == code) > 0;
    }

    /// <summary>
    /// Elimina lógicamente un tipo de cita estableciendo IsActive = false.
    /// </summary>
    /// <param name="id">Identificador único del tipo de cita a eliminar.</param>
    /// <returns>Task que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Este método implementa soft delete:
    /// - No elimina físicamente el registro de la base de datos
    /// - Establece IsActive = false para marcarlo como eliminado
    /// - Preserva el historial y las relaciones existentes
    /// - Si el tipo de cita no existe, no se realiza ninguna acción
    /// </remarks>
    public async Task DeleteAsync(int id)
    {
        var appointmentType = await GetByIdAsync(id);
        if (appointmentType != null)
        {
            appointmentType.IsActive = false;
        }
    }
}