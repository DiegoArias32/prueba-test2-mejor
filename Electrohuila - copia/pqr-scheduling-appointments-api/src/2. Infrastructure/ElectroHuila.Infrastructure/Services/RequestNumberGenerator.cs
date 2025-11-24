using ElectroHuila.Application.Common.Interfaces;
using ElectroHuila.Application.Common.Interfaces.Services.Common;

namespace ElectroHuila.Infrastructure.Services;

/// <summary>
/// Servicio para generar números únicos de solicitud.
/// Crea códigos con formato: REQ-FECHA-NUMEROS (ej: REQ-20251002-12345678)
/// </summary>
public class RequestNumberGenerator : IRequestNumberGenerator
{
    /// <summary>
    /// Genera un número único para una solicitud.
    /// Formato: REQ-YYYYMMDD-XXXXXXXX (8 dígitos aleatorios)
    /// </summary>
    /// <returns>Número de solicitud único (ej: REQ-20251002-12345678)</returns>
    public Task<string> GenerateAsync()
    {
        return Task.FromResult($"REQ-{System.DateTime.UtcNow:yyyyMMdd}-{GenerateRandomNumber(8)}");
    }

    /// <summary>
    /// Genera una secuencia de números aleatorios de la longitud especificada.
    /// </summary>
    /// <param name="length">Cantidad de dígitos a generar</param>
    /// <returns>Cadena de números aleatorios</returns>
    private static string GenerateRandomNumber(int length)
    {
        var random = new Random();
        var result = "";
        for (int i = 0; i < length; i++)
        {
            result += random.Next(0, 10).ToString();
        }
        return result;
    }
}