using ElectroHuila.Application.Common.Interfaces;
using ElectroHuila.Application.Common.Interfaces.Services.Common;

namespace ElectroHuila.Infrastructure.Services;

/// <summary>
/// Servicio para generar números únicos de cliente.
/// Crea códigos con formato: CLI-FECHA-NUMEROS (ej: CLI-20251002-123456)
/// </summary>
public class ClientNumberGenerator : IClientNumberGenerator
{
    /// <summary>
    /// Genera un número único para un cliente.
    /// Formato: CLI-YYYYMMDD-XXXXXX (6 dígitos aleatorios)
    /// </summary>
    /// <returns>Número de cliente único (ej: CLI-20251002-123456)</returns>
    public Task<string> GenerateAsync()
    {
        return Task.FromResult($"CLI-{System.DateTime.UtcNow:yyyyMMdd}-{GenerateRandomNumber(6)}");
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