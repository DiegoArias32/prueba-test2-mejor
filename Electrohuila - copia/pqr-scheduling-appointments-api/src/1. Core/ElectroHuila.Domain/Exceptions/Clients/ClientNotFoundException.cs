namespace ElectroHuila.Domain.Exceptions.Clients;

/// <summary>
/// Excepción que se lanza cuando no se encuentra un cliente
/// </summary>
public sealed class ClientNotFoundException : DomainException
{
    /// <summary>
    /// Constructor para excepción de cliente no encontrado por ID
    /// </summary>
    /// <param name="clientId">ID del cliente no encontrado</param>
    public ClientNotFoundException(int clientId)
        : base("CLIENT_NOT_FOUND", $"Client with ID {clientId} was not found.", new { ClientId = clientId })
    {
    }

    /// <summary>
    /// Constructor para excepción de cliente no encontrado por número
    /// </summary>
    /// <param name="clientNumber">Número del cliente no encontrado</param>
    public ClientNotFoundException(string clientNumber)
        : base("CLIENT_NOT_FOUND", $"Client with number {clientNumber} was not found.", new { ClientNumber = clientNumber })
    {
    }

    /// <summary>
    /// Constructor para excepción de cliente no encontrado por documento
    /// </summary>
    /// <param name="documentType">Tipo de documento</param>
    /// <param name="documentNumber">Número de documento</param>
    public ClientNotFoundException(string documentType, string documentNumber)
        : base("CLIENT_NOT_FOUND",
               $"Client with document {documentType} {documentNumber} was not found.",
               new { DocumentType = documentType, DocumentNumber = documentNumber })
    {
    }
}