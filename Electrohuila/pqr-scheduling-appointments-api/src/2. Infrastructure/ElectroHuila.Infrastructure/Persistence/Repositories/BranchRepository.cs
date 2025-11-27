using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Locations;
using ElectroHuila.Domain.Entities.Clients;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio para la gestión de sucursales de ElectroHuila en la base de datos.
/// Hereda de <see cref="BaseRepository{T}"/> e implementa <see cref="IBranchRepository"/>.
/// </summary>
/// <remarks>
/// Este repositorio proporciona operaciones específicas para la entidad <see cref="Branch"/>,
/// incluyendo búsquedas por ubicación geográfica, códigos identificadores, validaciones de existencia
/// y gestión de la sucursal principal. Todas las consultas filtran automáticamente por sucursales activas.
/// </remarks>
public class BranchRepository : BaseRepository<Branch>, IBranchRepository
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="BranchRepository"/>.
    /// </summary>
    /// <param name="context">Contexto de la base de datos de la aplicación.</param>
    public BranchRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Busca una sucursal por su nombre exacto entre las sucursales activas.
    /// </summary>
    /// <param name="name">Nombre de la sucursal a buscar.</param>
    /// <returns>
    /// La sucursal encontrada si existe y está activa; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// La búsqueda es sensible a mayúsculas y minúsculas según la configuración de la base de datos.
    /// Solo considera sucursales activas (IsActive = true).
    /// </remarks>
    public async Task<Branch?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(b => b.Name == name && b.IsActive);
    }

    /// <summary>
    /// Busca una sucursal por su código identificador único entre las sucursales activas.
    /// </summary>
    /// <param name="code">Código único de la sucursal a buscar.</param>
    /// <returns>
    /// La sucursal encontrada si existe y está activa; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// El código debe ser único en el sistema para cada sucursal activa.
    /// </remarks>
    public async Task<Branch?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(b => b.Code == code && b.IsActive);
    }

    /// <summary>
    /// Obtiene todas las sucursales activas del sistema ordenadas alfabéticamente.
    /// </summary>
    /// <returns>
    /// Colección de sucursales activas ordenadas por nombre.
    /// </returns>
    /// <remarks>
    /// Solo incluye sucursales que tienen IsActive = true, ordenadas alfabéticamente para facilitar la presentación.
    /// </remarks>
    public async Task<IEnumerable<Branch>> GetActiveAsync()
    {
        return await _dbSet
            .Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todas las sucursales del sistema incluyendo inactivas ordenadas alfabéticamente.
    /// </summary>
    /// <returns>
    /// Colección de todas las sucursales (activas e inactivas) ordenadas por nombre.
    /// </returns>
    /// <remarks>
    /// Incluye sucursales con IsActive = true e IsActive = false, ordenadas alfabéticamente.
    /// </remarks>
    public async Task<IEnumerable<Branch>> GetAllIncludingInactiveAsync()
    {
        return await _dbSet
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todas las sucursales activas ubicadas en una ciudad específica.
    /// </summary>
    /// <param name="city">Nombre de la ciudad donde buscar sucursales.</param>
    /// <returns>
    /// Colección de sucursales activas en la ciudad especificada, ordenadas por nombre.
    /// </returns>
    /// <remarks>
    /// Útil para filtrar sucursales por ubicación geográfica y presentar opciones locales a los usuarios.
    /// </remarks>
    public async Task<IEnumerable<Branch>> GetByCityAsync(string city)
    {
        return await _dbSet
            .Where(b => b.City == city && b.IsActive)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todas las sucursales activas ubicadas en un departamento/estado específico.
    /// </summary>
    /// <param name="department">Nombre del departamento o estado donde buscar sucursales.</param>
    /// <returns>
    /// Colección de sucursales activas en el departamento especificado, ordenadas por nombre.
    /// </returns>
    /// <remarks>
    /// Utiliza el campo State de la entidad Branch para la búsqueda por departamento.
    /// Útil para agrupar sucursales por división administrativa superior.
    /// </remarks>
    public async Task<IEnumerable<Branch>> GetByDepartmentAsync(string department)
    {
        return await _dbSet
            .Where(b => b.State == department && b.IsActive)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si existe una sucursal activa con el nombre especificado.
    /// </summary>
    /// <param name="name">Nombre de la sucursal a verificar.</param>
    /// <returns>
    /// true si existe una sucursal activa con ese nombre; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// Útil para validaciones de unicidad antes de crear nuevas sucursales.
    /// Solo considera sucursales activas para permitir reutilización de nombres de sucursales eliminadas.
    /// </remarks>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await _dbSet.CountAsync(b => b.Name == name && b.IsActive) > 0;
    }

    /// <summary>
    /// Verifica si existe una sucursal activa con el código especificado.
    /// </summary>
    /// <param name="code">Código de la sucursal a verificar.</param>
    /// <returns>
    /// true si existe una sucursal activa con ese código; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// Esencial para mantener la unicidad de códigos en el sistema.
    /// Solo considera sucursales activas para permitir reutilización de códigos de sucursales eliminadas.
    /// </remarks>
    public async Task<bool> ExistsByCodeAsync(string code)
    {
        // Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
        return await _dbSet.CountAsync(b => b.Code == code && b.IsActive) > 0;
    }

    /// <summary>
    /// Obtiene la sucursal principal del sistema si existe y está activa.
    /// </summary>
    /// <returns>
    /// La sucursal marcada como principal si existe y está activa; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// La sucursal principal se identifica por el campo IsMain = true.
    /// Solo puede haber una sucursal principal activa en el sistema.
    /// Se utiliza para operaciones que requieren una sucursal de referencia por defecto.
    /// </remarks>
    public async Task<Branch?> GetMainBranchAsync()
    {
        return await _dbSet.FirstOrDefaultAsync(b => b.IsMain && b.IsActive);
    }

    /// <summary>
    /// Elimina lógicamente una sucursal estableciendo IsActive = false.
    /// </summary>
    /// <param name="id">Identificador único de la sucursal a eliminar.</param>
    /// <returns>Task que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Implementa soft delete para preservar el historial de sucursales y sus relaciones.
    /// La eliminación no afecta citas existentes programadas en esa sucursal.
    /// Si la sucursal no existe, no se realiza ninguna acción.
    /// 
    /// <para><strong>Consideraciones importantes:</strong></para>
    /// - Las citas existentes mantendrán la referencia a la sucursal
    /// - Los horarios disponibles de la sucursal también deben ser gestionados
    /// - Verificar dependencias antes de eliminar sucursales principales
    /// </remarks>
    public async Task DeleteAsync(int id)
    {
        var branch = await GetByIdAsync(id);
        if (branch != null)
        {
            branch.IsActive = false;
        }
    }
}