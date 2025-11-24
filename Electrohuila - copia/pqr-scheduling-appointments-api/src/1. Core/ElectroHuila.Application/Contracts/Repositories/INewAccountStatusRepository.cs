using ElectroHuila.Domain.Entities.Catalogs;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repositorio para los estados de nuevas cuentas
/// </summary>
public interface INewAccountStatusRepository : IBaseRepository<NewAccountStatus>
{
    /// <summary>
    /// Obtiene un estado por su código
    /// </summary>
    Task<NewAccountStatus?> GetByCodeAsync(string code);

    /// <summary>
    /// Obtiene todos los estados activos ordenados
    /// </summary>
    Task<IEnumerable<NewAccountStatus>> GetAllActiveOrderedAsync();

    /// <summary>
    /// Verifica si existe un estado con el código especificado
    /// </summary>
    Task<bool> ExistsByCodeAsync(string code);
}
