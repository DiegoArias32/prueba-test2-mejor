using ElectroHuila.Domain.Entities.Catalogs;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repositorio para los tipos de proyecto
/// </summary>
public interface IProjectTypeRepository : IBaseRepository<ProjectType>
{
    /// <summary>
    /// Obtiene un tipo de proyecto por su código
    /// </summary>
    Task<ProjectType?> GetByCodeAsync(string code);

    /// <summary>
    /// Obtiene todos los tipos de proyecto activos ordenados
    /// </summary>
    Task<IEnumerable<ProjectType>> GetAllActiveOrderedAsync();
}
