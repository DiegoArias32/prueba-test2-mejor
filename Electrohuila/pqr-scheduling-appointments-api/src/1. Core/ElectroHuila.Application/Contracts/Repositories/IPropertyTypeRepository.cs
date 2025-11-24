using ElectroHuila.Domain.Entities.Catalogs;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repositorio para los tipos de propiedad
/// </summary>
public interface IPropertyTypeRepository : IBaseRepository<PropertyType>
{
    /// <summary>
    /// Obtiene un tipo de propiedad por su código
    /// </summary>
    Task<PropertyType?> GetByCodeAsync(string code);

    /// <summary>
    /// Obtiene todos los tipos de propiedad activos ordenados
    /// </summary>
    Task<IEnumerable<PropertyType>> GetAllActiveOrderedAsync();
}
