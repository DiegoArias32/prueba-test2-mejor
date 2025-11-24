namespace ElectroHuila.Domain.Exceptions.Clients;

/// <summary>
/// Excepción que se lanza cuando se intenta registrar un número de documento duplicado
/// </summary>
public sealed class DuplicateDocumentNumberException : DomainException
{
    /// <summary>
    /// Constructor para excepción de número de documento duplicado
    /// </summary>
    /// <param name="documentType">Tipo de documento</param>
    /// <param name="documentNumber">Número de documento duplicado</param>
    public DuplicateDocumentNumberException(string documentType, string documentNumber)
        : base("DUPLICATE_DOCUMENT_NUMBER",
               $"A client with document {documentType} {documentNumber} already exists.",
               new { DocumentType = documentType, DocumentNumber = documentNumber })
    {
    }

    /// <summary>
    /// Constructor para excepción de número de documento duplicado con ID del cliente existente
    /// </summary>
    /// <param name="documentType">Tipo de documento</param>
    /// <param name="documentNumber">Número de documento duplicado</param>
    /// <param name="existingClientId">ID del cliente existente con ese documento</param>
    public DuplicateDocumentNumberException(string documentType, string documentNumber, int existingClientId)
        : base("DUPLICATE_DOCUMENT_NUMBER",
               $"A client with document {documentType} {documentNumber} already exists (Client ID: {existingClientId}).",
               new { DocumentType = documentType, DocumentNumber = documentNumber, ExistingClientId = existingClientId })
    {
    }
}