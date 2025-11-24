namespace ElectroHuila.Application.DTOs.Clients;

/// <summary>
/// Data transfer object for creating a new client.
/// Used when registering a new client in the system.
/// </summary>
public class CreateClientDto
{
    /// <summary>
    /// The type of identification document (e.g., ID card, passport).
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// The client's identification document number.
    /// </summary>
    public string DocumentNumber { get; set; } = string.Empty;

    /// <summary>
    /// The full name of the client.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// The client's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The client's landline phone number.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// The client's mobile phone number.
    /// </summary>
    public string Mobile { get; set; } = string.Empty;

    /// <summary>
    /// The client's physical address.
    /// </summary>
    public string Address { get; set; } = string.Empty;
}