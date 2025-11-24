using ElectroHuila.Application.Common.Interfaces;
using ElectroHuila.Application.Common.Interfaces.Services.Common;

namespace ElectroHuila.Infrastructure.Services;

/// <summary>
/// Servicio para generar números únicos de citas, clientes y solicitudes.
/// Crea códigos con formato: PREFIJO-FECHA-CODIGO (ej: APT-20251002-A1B2C3D4)
/// </summary>
public class AppointmentNumberGenerator : IAppointmentNumberGenerator
{
    /// <summary>
    /// Genera un número único para una cita.
    /// Formato: APT-YYYYMMDD-XXXXXXXX
    /// </summary>
    /// <returns>Número de cita único (ej: APT-20251002-A1B2C3D4)</returns>
    public Task<string> GenerateAppointmentNumberAsync()
    {
        return Task.FromResult($"APT-{System.DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}");
    }

    /// <summary>
    /// Genera un número único para un cliente.
    /// Formato: CLI-YYYYMMDD-XXXXXXXX
    /// </summary>
    /// <returns>Número de cliente único (ej: CLI-20251002-B2C3D4E5)</returns>
    public Task<string> GenerateClientNumberAsync()
    {
        return Task.FromResult($"CLI-{System.DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}");
    }

    /// <summary>
    /// Genera un número único para una solicitud.
    /// Formato: REQ-YYYYMMDD-XXXXXXXX
    /// </summary>
    /// <returns>Número de solicitud único (ej: REQ-20251002-C3D4E5F6)</returns>
    public Task<string> GenerateRequestNumberAsync()
    {
        return Task.FromResult($"REQ-{System.DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}");
    }

    /// <summary>
    /// Valida si un número de cita tiene el formato correcto.
    /// Debe empezar con "APT-" y tener al menos 3 partes separadas por guiones.
    /// </summary>
    /// <param name="appointmentNumber">Número de cita a validar</param>
    /// <returns>true si el formato es válido, false si no</returns>
    public bool IsValidAppointmentNumber(string appointmentNumber)
    {
        if (string.IsNullOrWhiteSpace(appointmentNumber))
            return false;

        var parts = appointmentNumber.Split('-');
        return parts.Length >= 3 && parts[0] == "APT";
    }

    /// <summary>
    /// Valida si un número de cliente tiene el formato correcto.
    /// Debe empezar con "CLI-" y tener al menos 3 partes separadas por guiones.
    /// </summary>
    /// <param name="clientNumber">Número de cliente a validar</param>
    /// <returns>true si el formato es válido, false si no</returns>
    public bool IsValidClientNumber(string clientNumber)
    {
        if (string.IsNullOrWhiteSpace(clientNumber))
            return false;

        var parts = clientNumber.Split('-');
        return parts.Length >= 3 && parts[0] == "CLI";
    }

    /// <summary>
    /// Valida si un número de solicitud tiene el formato correcto.
    /// Debe empezar con "REQ-" y tener al menos 3 partes separadas por guiones.
    /// </summary>
    /// <param name="requestNumber">Número de solicitud a validar</param>
    /// <returns>true si el formato es válido, false si no</returns>
    public bool IsValidRequestNumber(string requestNumber)
    {
        if (string.IsNullOrWhiteSpace(requestNumber))
            return false;

        var parts = requestNumber.Split('-');
        return parts.Length >= 3 && parts[0] == "REQ";
    }
}