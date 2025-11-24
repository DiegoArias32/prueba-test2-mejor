namespace ElectroHuila.Domain.ValueObjects;

/// <summary>
/// Value Object que representa una dirección física
/// </summary>
public record Address
{
    /// <summary>
    /// Nombre de la calle
    /// </summary>
    public string Street { get; }

    /// <summary>
    /// Nombre de la ciudad
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Nombre del departamento o estado
    /// </summary>
    public string State { get; }

    /// <summary>
    /// Código postal (opcional)
    /// </summary>
    public string? PostalCode { get; }

    /// <summary>
    /// Dirección completa formateada
    /// </summary>
    public string FullAddress => $"{Street}, {City}, {State}" + (PostalCode != null ? $" {PostalCode}" : "");

    /// <summary>
    /// Constructor privado para crear una instancia de dirección
    /// </summary>
    private Address(string street, string city, string state, string? postalCode = null)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
    }

    /// <summary>
    /// Crea una instancia validada de dirección
    /// </summary>
    /// <param name="street">Nombre de la calle</param>
    /// <param name="city">Nombre de la ciudad</param>
    /// <param name="state">Nombre del departamento</param>
    /// <param name="postalCode">Código postal (opcional)</param>
    /// <returns>Nueva instancia de dirección validada</returns>
    public static Address Create(string street, string city, string state, string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be null or empty.", nameof(street));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be null or empty.", nameof(city));

        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State cannot be null or empty.", nameof(state));

        var trimmedStreet = street.Trim();
        var trimmedCity = city.Trim();
        var trimmedState = state.Trim();
        var trimmedPostalCode = postalCode?.Trim();

        if (trimmedStreet.Length < 5)
            throw new ArgumentException("Street must be at least 5 characters long.", nameof(street));

        if (trimmedCity.Length < 2)
            throw new ArgumentException("City must be at least 2 characters long.", nameof(city));

        if (trimmedState.Length < 2)
            throw new ArgumentException("State must be at least 2 characters long.", nameof(state));

        return new Address(trimmedStreet, trimmedCity, trimmedState, trimmedPostalCode);
    }

    /// <summary>
    /// Crea una instancia de dirección a partir de una cadena de dirección completa
    /// </summary>
    /// <param name="fullAddress">Dirección completa en formato: calle, ciudad, estado [código postal]</param>
    /// <returns>Nueva instancia de dirección validada</returns>
    public static Address CreateFromFullAddress(string fullAddress)
    {
        if (string.IsNullOrWhiteSpace(fullAddress))
            throw new ArgumentException("Full address cannot be null or empty.", nameof(fullAddress));

        // Simple parsing - in a real application, you might want to use a more sophisticated address parser
        var parts = fullAddress.Split(',', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            throw new ArgumentException("Full address must contain at least street, city, and state separated by commas.", nameof(fullAddress));

        var street = parts[0].Trim();
        var city = parts[1].Trim();
        var stateAndPostal = parts[2].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var state = stateAndPostal[0];
        var postalCode = stateAndPostal.Length > 1 ? stateAndPostal[1] : null;

        return Create(street, city, state, postalCode);
    }

    /// <summary>
    /// Conversión implícita de Address a string
    /// </summary>
    public static implicit operator string(Address address) => address.FullAddress;

    /// <summary>
    /// Retorna la dirección completa formateada
    /// </summary>
    public override string ToString() => FullAddress;
}