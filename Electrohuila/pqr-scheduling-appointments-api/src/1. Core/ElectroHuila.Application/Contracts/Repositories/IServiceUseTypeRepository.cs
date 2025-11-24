using ElectroHuila.Domain.Entities.Catalogs;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repositorio para los tipos de uso de servicio
/// </summary>
public interface IServiceUseTypeRepository : IBaseRepository<ServiceUseType>
{
    /// <summary>
    /// Obtiene un tipo de uso por su código
    /// </summary>
    Task<ServiceUseType?> GetByCodeAsync(string code);

    /// <summary>
    /// Obtiene todos los tipos de uso activos ordenados
    /// </summary>
    Task<IEnumerable<ServiceUseType>> GetAllActiveOrderedAsync();
}
