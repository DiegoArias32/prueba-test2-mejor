namespace ElectroHuila.Application.DTOs.Clients;

/// <summary>
/// Data transfer object for updating an existing client.
/// Used when modifying client information such as contact details or identification.
/// </summary>
public class UpdateClientDto
{
    /// <summary>
    /// The updated type of identification document.
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// The updated identification document number.
    /// </summary>
    public string DocumentNumber { get; set; } = string.Empty;

    /// <summary>
    /// The updated full name of the client.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// The updated email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The updated landline phone number.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// The updated mobile phone number.
    /// </summary>
    public string Mobile { get; set; } = string.Empty;

    /// <summary>
    /// The updated physical address.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the client is active.
    /// If null, the current value will be preserved.
    /// </summary>
    public bool? IsActive { get; set; }
}