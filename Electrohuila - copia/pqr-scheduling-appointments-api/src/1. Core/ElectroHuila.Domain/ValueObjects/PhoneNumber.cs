using System.Text.RegularExpressions;

namespace ElectroHuila.Domain.ValueObjects;

/// <summary>
/// Value Object que representa un número de teléfono colombiano válido
/// </summary>
public record PhoneNumber
{
    /// <summary>
    /// Expresión regular para validar números de teléfono colombianos
    /// </summary>
    private static readonly Regex PhoneRegex = new(
        @"^(\+57\s?)?([1-9]\d{6,9})$",
        RegexOptions.Compiled);

    /// <summary>
    /// Expresión regular para validar números de teléfono móvil colombianos
    /// </summary>
    private static readonly Regex MobileRegex = new(
        @"^(\+57\s?)?(3\d{9})$",
        RegexOptions.Compiled);

    /// <summary>
    /// Valor del número de teléfono
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Indica si es un número móvil
    /// </summary>
    public bool IsMobile { get; }

    /// <summary>
    /// Número de teléfono formateado
    /// </summary>
    public string FormattedValue { get; }

    /// <summary>
    /// Constructor privado para crear una instancia de número de teléfono
    /// </summary>
    private PhoneNumber(string value, bool isMobile)
    {
        Value = value;
        IsMobile = isMobile;
        FormattedValue = FormatPhoneNumber(value, isMobile);
    }

    /// <summary>
    /// Crea una instancia validada de número de teléfono colombiano
    /// </summary>
    /// <param name="phoneNumber">Número de teléfono a validar</param>
    /// <param name="validateAsMobile">Indica si se debe validar específicamente como número móvil</param>
    /// <returns>Nueva instancia de número de teléfono validada</returns>
    public static PhoneNumber Create(string phoneNumber, bool validateAsMobile = false)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be null or empty.", nameof(phoneNumber));

        var cleanedNumber = CleanPhoneNumber(phoneNumber);

        if (validateAsMobile)
        {
            if (!MobileRegex.IsMatch(cleanedNumber))
                throw new ArgumentException("Invalid mobile phone number format. Colombian mobile numbers must start with 3 and have 10 digits.", nameof(phoneNumber));

            return new PhoneNumber(cleanedNumber, true);
        }

        if (!PhoneRegex.IsMatch(cleanedNumber))
            throw new ArgumentException("Invalid phone number format. Colombian phone numbers must have 7-10 digits.", nameof(phoneNumber));

        var isMobile = MobileRegex.IsMatch(cleanedNumber);
        return new PhoneNumber(cleanedNumber, isMobile);
    }

    /// <summary>
    /// Limpia el número de teléfono removiendo caracteres no numéricos y manejando código de país
    /// </summary>
    private static string CleanPhoneNumber(string phoneNumber)
    {
        // Remove all non-digit characters except +
        var cleaned = Regex.Replace(phoneNumber, @"[^\d+]", "");

        // Handle Colombian country code
        if (cleaned.StartsWith("+57"))
        {
            cleaned = cleaned.Substring(3);
        }
        else if (cleaned.StartsWith("57") && cleaned.Length > 10)
        {
            cleaned = cleaned.Substring(2);
        }

        return cleaned;
    }

    /// <summary>
    /// Formatea el número de teléfono para presentación
    /// </summary>
    private static string FormatPhoneNumber(string phoneNumber, bool isMobile)
    {
        if (isMobile && phoneNumber.Length == 10)
        {
            // Format: 3XX XXX XXXX
            return $"{phoneNumber.Substring(0, 3)} {phoneNumber.Substring(3, 3)} {phoneNumber.Substring(6)}";
        }

        if (!isMobile && phoneNumber.Length >= 7)
        {
            // Format: XXX XXXX or XXXX XXXX
            if (phoneNumber.Length == 7)
                return $"{phoneNumber.Substring(0, 3)} {phoneNumber.Substring(3)}";
            else
                return $"{phoneNumber.Substring(0, phoneNumber.Length - 4)} {phoneNumber.Substring(phoneNumber.Length - 4)}";
        }

        return phoneNumber;
    }

    /// <summary>
    /// Conversión implícita de PhoneNumber a string
    /// </summary>
    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;

    /// <summary>
    /// Retorna el número de teléfono formateado
    /// </summary>
    public override string ToString() => FormattedValue;
}