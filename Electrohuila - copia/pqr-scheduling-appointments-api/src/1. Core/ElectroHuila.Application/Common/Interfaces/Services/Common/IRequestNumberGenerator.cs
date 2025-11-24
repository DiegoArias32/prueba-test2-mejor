namespace ElectroHuila.Application.Common.Interfaces.Services.Common;

/// <summary>
/// Servicio para generar números únicos de solicitud
/// </summary>
public interface IRequestNumberGenerator
{
    /// <summary>
    /// Genera un número único para una nueva solicitud
    /// </summary>
    /// <returns>Número de solicitud generado con formato específico</returns>
    Task<string> GenerateAsync();
}
