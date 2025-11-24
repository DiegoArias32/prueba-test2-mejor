using System.Text.RegularExpressions;

namespace ElectroHuila.Domain.ValueObjects;

/// <summary>
/// Value Object que representa un número de documento de identidad válido
/// </summary>
public record DocumentNumber
{
    /// <summary>
    /// Validadores de formato por tipo de documento
    /// </summary>
    private static readonly Dictionary<string, Regex> DocumentValidators = new()
    {
        { "CC", new Regex(@"^\d{6,10}$", RegexOptions.Compiled) }, // Cédula de Ciudadanía
        { "CE", new Regex(@"^\d{6,12}$", RegexOptions.Compiled) }, // Cédula de Extranjería
        { "TI", new Regex(@"^\d{8,11}$", RegexOptions.Compiled) }, // Tarjeta de Identidad
        { "PAS", new Regex(@"^[A-Z0-9]{6,12}$", RegexOptions.Compiled) }, // Pasaporte
        { "NIT", new Regex(@"^\d{8,15}$", RegexOptions.Compiled) } // NIT
    };

    /// <summary>
    /// Valor del número de documento
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Tipo de documento
    /// </summary>
    public string DocumentType { get; }

    /// <summary>
    /// Constructor privado para crear una instancia de número de documento
    /// </summary>
    private DocumentNumber(string value, string documentType)
    {
        Value = value;
        DocumentType = documentType;
    }

    /// <summary>
    /// Crea una instancia validada de número de documento
    /// </summary>
    /// <param name="documentNumber">Número de documento</param>
    /// <param name="documentType">Tipo de documento (CC, CE, TI, PAS, NIT)</param>
    /// <returns>Nueva instancia de número de documento validada</returns>
    public static DocumentNumber Create(string documentNumber, string documentType)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new ArgumentException("Document number cannot be null or empty.", nameof(documentNumber));

        if (string.IsNullOrWhiteSpace(documentType))
            throw new ArgumentException("Document type cannot be null or empty.", nameof(documentType));

        var trimmedNumber = documentNumber.Trim().ToUpperInvariant();
        var trimmedType = documentType.Trim().ToUpperInvariant();

        if (!DocumentValidators.ContainsKey(trimmedType))
            throw new ArgumentException($"Invalid document type: {documentType}", nameof(documentType));

        if (!DocumentValidators[trimmedType].IsMatch(trimmedNumber))
            throw new ArgumentException($"Invalid document number format for type {documentType}.", nameof(documentNumber));

        return new DocumentNumber(trimmedNumber, trimmedType);
    }

    /// <summary>
    /// Conversión implícita de DocumentNumber a string
    /// </summary>
    public static implicit operator string(DocumentNumber documentNumber) => documentNumber.Value;

    /// <summary>
    /// Retorna el número de documento con su tipo en formato legible
    /// </summary>
    public override string ToString() => $"{DocumentType}: {Value}";
}