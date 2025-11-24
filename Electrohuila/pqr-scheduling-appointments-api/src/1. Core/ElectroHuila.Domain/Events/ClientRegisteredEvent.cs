namespace ElectroHuila.Domain.Events;

/// <summary>
/// Evento que se dispara cuando se registra un nuevo cliente
/// </summary>
public sealed record ClientRegisteredEvent : DomainEvent
{
    /// <summary>
    /// Tipo de evento
    /// </summary>
    public override string EventType => "ClientRegistered";

    /// <summary>
    /// Identificador del cliente registrado
    /// </summary>
    public int ClientId { get; init; }

    /// <summary>
    /// Número único asignado al cliente
    /// </summary>
    public string ClientNumber { get; init; } = string.Empty;

    /// <summary>
    /// Tipo de documento de identidad del cliente
    /// </summary>
    public string DocumentType { get; init; } = string.Empty;

    /// <summary>
    /// Número de documento de identidad del cliente
    /// </summary>
    public string DocumentNumber { get; init; } = string.Empty;

    /// <summary>
    /// Nombre completo del cliente
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Correo electrónico del cliente
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Número de teléfono fijo del cliente
    /// </summary>
    public string Phone { get; init; } = string.Empty;

    /// <summary>
    /// Número de teléfono móvil del cliente
    /// </summary>
    public string Mobile { get; init; } = string.Empty;

    /// <summary>
    /// Dirección física del cliente
    /// </summary>
    public string Address { get; init; } = string.Empty;

    /// <summary>
    /// Usuario que registró al cliente en el sistema
    /// </summary>
    public string RegisteredBy { get; init; } = string.Empty;

    /// <summary>
    /// Constructor para crear el evento de cliente registrado
    /// </summary>
    public ClientRegisteredEvent(
        int clientId,
        string clientNumber,
        string documentType,
        string documentNumber,
        string fullName,
        string email,
        string phone,
        string mobile,
        string address,
        string registeredBy)
    {
        ClientId = clientId;
        ClientNumber = clientNumber;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        FullName = fullName;
        Email = email;
        Phone = phone;
        Mobile = mobile;
        Address = address;
        RegisteredBy = registeredBy;
    }
}