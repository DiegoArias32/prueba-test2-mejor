namespace ElectroHuila.Application.Common.Interfaces.Services.Common;

/// <summary>
/// Servicio para generar y validar números únicos de citas, clientes y solicitudes
/// </summary>
public interface IAppointmentNumberGenerator
{
    /// <summary>
    /// Genera un número único para una nueva cita
    /// </summary>
    /// <returns>Número de cita generado con formato específico</returns>
    Task<string> GenerateAppointmentNumberAsync();

    /// <summary>
    /// Genera un número único para un nuevo cliente
    /// </summary>
    /// <returns>Número de cliente generado con formato específico</returns>
    Task<string> GenerateClientNumberAsync();

    /// <summary>
    /// Genera un número único para una nueva solicitud
    /// </summary>
    /// <returns>Número de solicitud generado con formato específico</returns>
    Task<string> GenerateRequestNumberAsync();

    /// <summary>
    /// Valida si un número de cita tiene el formato correcto
    /// </summary>
    /// <param name="appointmentNumber">Número de cita a validar</param>
    /// <returns>True si el formato es válido</returns>
    bool IsValidAppointmentNumber(string appointmentNumber);

    /// <summary>
    /// Valida si un número de cliente tiene el formato correcto
    /// </summary>
    /// <param name="clientNumber">Número de cliente a validar</param>
    /// <returns>True si el formato es válido</returns>
    bool IsValidClientNumber(string clientNumber);

    /// <summary>
    /// Valida si un número de solicitud tiene el formato correcto
    /// </summary>
    /// <param name="requestNumber">Número de solicitud a validar</param>
    /// <returns>True si el formato es válido</returns>
    bool IsValidRequestNumber(string requestNumber);
}