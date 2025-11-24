namespace ElectroHuila.Application.Common.Interfaces.Services.Common;

/// <summary>
/// Servicio para generar números únicos de cliente
/// </summary>
public interface IClientNumberGenerator
{
    /// <summary>
    /// Genera un número único para un nuevo cliente
    /// </summary>
    /// <returns>Número de cliente generado con formato específico</returns>
    Task<string> GenerateAsync();
}
