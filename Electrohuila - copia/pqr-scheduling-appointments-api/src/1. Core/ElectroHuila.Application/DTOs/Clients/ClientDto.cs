namespace ElectroHuila.Application.DTOs.Clients;

/// <summary>
/// Data transfer object representing client information in the system.
/// Contains complete details about a registered client including contact information.
/// </summary>
public class ClientDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the client.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique client number assigned by the system.
    /// Used for client identification and tracking across the application.
    /// </summary>
    public string ClientNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of identification document (e.g., CC, TI, CE, NIT).
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identification document number of the client.
    /// </summary>
    public string DocumentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name of the client.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the client.
    /// Used for notifications and communication.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the landline phone number of the client.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the mobile phone number of the client.
    /// </summary>
    public string Mobile { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the physical address of the client.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the client record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the last update to the client record.
    /// Null if the record has never been updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the client is active in the system.
    /// </summary>
    public bool IsActive { get; set; }
}