using ElectroHuila.Application.Common.Models;

namespace ElectroHuila.Application.Common.Interfaces.Services.External;

/// <summary>
/// Servicio para enviar mensajes SMS
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// Envía un mensaje SMS a un número telefónico
    /// </summary>
    /// <param name="phoneNumber">Número de teléfono del destinatario</param>
    /// <param name="message">Contenido del mensaje</param>
    /// <returns>Resultado indicando si el envío fue exitoso</returns>
    Task<Result<bool>> SendSmsAsync(string phoneNumber, string message);
}
