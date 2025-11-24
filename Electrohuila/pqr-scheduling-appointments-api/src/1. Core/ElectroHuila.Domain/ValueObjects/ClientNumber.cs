using System.Text.RegularExpressions;

namespace ElectroHuila.Domain.ValueObjects;

/// <summary>
/// Value Object que representa un número de cliente único
/// </summary>
public record ClientNumber
{
    /// <summary>
    /// Expresión regular para validar el formato del número de cliente
    /// </summary>
    private static readonly Regex ClientNumberRegex = new(
        @"^CLI-\d{8}-[A-F0-9]{32}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Valor del número de cliente
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Constructor privado para crear una instancia de número de cliente
    /// </summary>
    private ClientNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Crea una instancia validada de número de cliente desde una cadena existente
    /// </summary>
    /// <param name="clientNumber">Número de cliente en formato CLI-YYYYMMDD-{GUID}</param>
    /// <returns>Nueva instancia de número de cliente validada</returns>
    public static ClientNumber Create(string clientNumber)
    {
        if (string.IsNullOrWhiteSpace(clientNumber))
            throw new ArgumentException("Client number cannot be null or empty.", nameof(clientNumber));

        var trimmedNumber = clientNumber.Trim().ToUpperInvariant();

        if (!ClientNumberRegex.IsMatch(trimmedNumber))
            throw new ArgumentException("Invalid client number format. Expected format: CLI-YYYYMMDD-{GUID}", nameof(clientNumber));

        return new ClientNumber(trimmedNumber);
    }

    /// <summary>
    /// Genera un nuevo número de cliente único
    /// </summary>
    /// <returns>Nueva instancia de número de cliente generada automáticamente</returns>
    public static ClientNumber Generate()
    {
        var datePrefix = DateTime.UtcNow.ToString("yyyyMMdd");
        var guid = Guid.NewGuid().ToString("N").ToUpperInvariant();
        var clientNumber = $"CLI-{datePrefix}-{guid}";

        return new ClientNumber(clientNumber);
    }

    /// <summary>
    /// Conversión implícita de ClientNumber a string
    /// </summary>
    public static implicit operator string(ClientNumber clientNumber) => clientNumber.Value;

    /// <summary>
    /// Retorna el valor del número de cliente
    /// </summary>
    public override string ToString() => Value;
}