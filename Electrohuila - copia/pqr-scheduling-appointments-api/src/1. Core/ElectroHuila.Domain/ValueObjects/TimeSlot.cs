using System.Text.RegularExpressions;

namespace ElectroHuila.Domain.ValueObjects;

/// <summary>
/// Value Object que representa un horario válido para citas
/// </summary>
public record TimeSlot
{
    /// <summary>
    /// Expresión regular para validar el formato de hora
    /// </summary>
    private static readonly Regex TimeRegex = new(
        @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$",
        RegexOptions.Compiled);

    /// <summary>
    /// Valor del horario en formato string
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Valor del horario en formato TimeOnly
    /// </summary>
    public TimeOnly Time { get; }

    /// <summary>
    /// Constructor privado para crear una instancia de horario
    /// </summary>
    private TimeSlot(string value, TimeOnly time)
    {
        Value = value;
        Time = time;
    }

    /// <summary>
    /// Crea una instancia validada de horario desde una cadena
    /// </summary>
    /// <param name="timeSlot">Horario en formato HH:MM</param>
    /// <returns>Nueva instancia de horario validada</returns>
    public static TimeSlot Create(string timeSlot)
    {
        if (string.IsNullOrWhiteSpace(timeSlot))
            throw new ArgumentException("Time slot cannot be null or empty.", nameof(timeSlot));

        var trimmedTime = timeSlot.Trim();

        if (!TimeRegex.IsMatch(trimmedTime))
            throw new ArgumentException("Invalid time format. Expected format: HH:MM", nameof(timeSlot));

        if (!TimeOnly.TryParse(trimmedTime, out var time))
            throw new ArgumentException("Invalid time value.", nameof(timeSlot));

        // Validate business hours (8:00 AM to 6:00 PM)
        if (time < new TimeOnly(8, 0) || time > new TimeOnly(18, 0))
            throw new ArgumentException("Time slot must be between 08:00 and 18:00.", nameof(timeSlot));

        // Validate 30-minute intervals
        if (time.Minute % 30 != 0)
            throw new ArgumentException("Time slot must be in 30-minute intervals (00 or 30 minutes).", nameof(timeSlot));

        return new TimeSlot(trimmedTime, time);
    }

    /// <summary>
    /// Crea una instancia validada de horario desde un TimeOnly
    /// </summary>
    /// <param name="time">Horario como TimeOnly</param>
    /// <returns>Nueva instancia de horario validada</returns>
    public static TimeSlot Create(TimeOnly time)
    {
        return Create(time.ToString("HH:mm"));
    }

    /// <summary>
    /// Conversión implícita de TimeSlot a string
    /// </summary>
    public static implicit operator string(TimeSlot timeSlot) => timeSlot.Value;

    /// <summary>
    /// Conversión implícita de TimeSlot a TimeOnly
    /// </summary>
    public static implicit operator TimeOnly(TimeSlot timeSlot) => timeSlot.Time;

    /// <summary>
    /// Retorna el valor del horario
    /// </summary>
    public override string ToString() => Value;
}