using System.Text.RegularExpressions;

namespace ElectroHuila.Domain.ValueObjects;

/// <summary>
/// Value Object que representa un número de cita único
/// </summary>
public record AppointmentNumber
{
    /// <summary>
    /// Expresión regular para validar el formato del número de cita
    /// </summary>
    private static readonly Regex AppointmentNumberRegex = new(
        @"^APT-\d{8}-[A-F0-9]{32}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Valor del número de cita
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Constructor privado para crear una instancia de número de cita
    /// </summary>
    private AppointmentNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Crea una instancia validada de número de cita desde una cadena existente
    /// </summary>
    /// <param name="appointmentNumber">Número de cita en formato APT-YYYYMMDD-{GUID}</param>
    /// <returns>Nueva instancia de número de cita validada</returns>
    public static AppointmentNumber Create(string appointmentNumber)
    {
        if (string.IsNullOrWhiteSpace(appointmentNumber))
            throw new ArgumentException("Appointment number cannot be null or empty.", nameof(appointmentNumber));

        var trimmedNumber = appointmentNumber.Trim().ToUpperInvariant();

        if (!AppointmentNumberRegex.IsMatch(trimmedNumber))
            throw new ArgumentException("Invalid appointment number format. Expected format: APT-YYYYMMDD-{GUID}", nameof(appointmentNumber));

        return new AppointmentNumber(trimmedNumber);
    }

    /// <summary>
    /// Genera un nuevo número de cita único
    /// </summary>
    /// <returns>Nueva instancia de número de cita generada automáticamente</returns>
    public static AppointmentNumber Generate()
    {
        var datePrefix = DateTime.UtcNow.ToString("yyyyMMdd");
        var guid = Guid.NewGuid().ToString("N").ToUpperInvariant();
        var appointmentNumber = $"APT-{datePrefix}-{guid}";

        return new AppointmentNumber(appointmentNumber);
    }

    /// <summary>
    /// Conversión implícita de AppointmentNumber a string
    /// </summary>
    public static implicit operator string(AppointmentNumber appointmentNumber) => appointmentNumber.Value;

    /// <summary>
    /// Retorna el valor del número de cita
    /// </summary>
    public override string ToString() => Value;
}