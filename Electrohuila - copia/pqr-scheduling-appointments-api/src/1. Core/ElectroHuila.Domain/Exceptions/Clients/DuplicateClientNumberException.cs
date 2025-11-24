namespace ElectroHuila.Domain.Exceptions.Clients;

/// <summary>
/// Excepción que se lanza cuando se intenta registrar un número de cliente duplicado
/// </summary>
public sealed class DuplicateClientNumberException : DomainException
{
    /// <summary>
    /// Constructor para excepción de número de cliente duplicado
    /// </summary>
    /// <param name="clientNumber">Número de cliente duplicado</param>
    public DuplicateClientNumberException(string clientNumber)
        : base("DUPLICATE_CLIENT_NUMBER",
               $"A client with number {clientNumber} already exists.",
               new { ClientNumber = clientNumber })
    {
    }

    /// <summary>
    /// Constructor para excepción de número de cliente duplicado con ID del cliente existente
    /// </summary>
    /// <param name="clientNumber">Número de cliente duplicado</param>
    /// <param name="existingClientId">ID del cliente existente con ese número</param>
    public DuplicateClientNumberException(string clientNumber, int existingClientId)
        : base("DUPLICATE_CLIENT_NUMBER",
               $"A client with number {clientNumber} already exists (Client ID: {existingClientId}).",
               new { ClientNumber = clientNumber, ExistingClientId = existingClientId })
    {
    }
}