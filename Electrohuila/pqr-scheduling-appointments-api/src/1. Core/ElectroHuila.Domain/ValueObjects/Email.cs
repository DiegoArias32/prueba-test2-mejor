using System.Text.RegularExpressions;

namespace ElectroHuila.Domain.ValueObjects;

/// <summary>
/// Value Object que representa un correo electrónico válido
/// </summary>
public record Email
{
    /// <summary>
    /// Expresión regular para validar el formato del correo electrónico
    /// </summary>
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Valor del correo electrónico
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Constructor privado para crear una instancia de correo electrónico
    /// </summary>
    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Crea una instancia validada de correo electrónico
    /// </summary>
    /// <param name="email">Dirección de correo electrónico a validar</param>
    /// <returns>Nueva instancia de correo electrónico validada</returns>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));

        var trimmedEmail = email.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(trimmedEmail))
            throw new ArgumentException("Invalid email format.", nameof(email));

        return new Email(trimmedEmail);
    }

    /// <summary>
    /// Conversión implícita de Email a string
    /// </summary>
    public static implicit operator string(Email email) => email.Value;

    /// <summary>
    /// Retorna el valor del correo electrónico
    /// </summary>
    public override string ToString() => Value;
}