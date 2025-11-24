using ElectroHuila.Application.Common.Models;

namespace ElectroHuila.Application.Common.Interfaces.Services.External;

/// <summary>
/// Servicio para integración con la API externa de ElectroHuila
/// </summary>
public interface IElectroHuilaApiService
{
    /// <summary>
    /// Obtiene la información de un cliente desde la API de ElectroHuila
    /// </summary>
    /// <param name="documentNumber">Número de documento del cliente</param>
    /// <returns>Resultado con la información del cliente</returns>
    Task<Result<dynamic>> GetClientInfoAsync(string documentNumber);

    /// <summary>
    /// Valida una cuenta de cliente en la API de ElectroHuila
    /// </summary>
    /// <param name="accountNumber">Número de cuenta a validar</param>
    /// <returns>Resultado con la información de validación</returns>
    Task<Result<dynamic>> ValidateAccountAsync(string accountNumber);
}
