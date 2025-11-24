using ElectroHuila.Domain.Entities.Common;
using ElectroHuila.Domain.Entities.Appointments;
using ElectroHuila.Domain.Enums;

namespace ElectroHuila.Domain.Entities.Clients;

/// <summary>
/// Representa un cliente registrado en el sistema
/// </summary>
public class Client : BaseEntity
{
    /// <summary>
    /// Número único del cliente generado por el sistema
    /// </summary>
    public string ClientNumber { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de documento de identidad del cliente (CC, CE, NIT, etc.)
    /// </summary>
    public DocumentType DocumentType { get; set; }

    /// <summary>
    /// Número de documento de identidad del cliente
    /// </summary>
    public string DocumentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del cliente
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del cliente
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Número de teléfono fijo del cliente
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Número de teléfono móvil del cliente
    /// </summary>
    public string Mobile { get; set; } = string.Empty;

    /// <summary>
    /// Dirección física del cliente
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Colección de citas asociadas al cliente
    /// </summary>
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}