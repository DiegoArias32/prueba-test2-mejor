using ElectroHuila.Application.DTOs.Branches;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using ElectroHuila.Application.DTOs.Clients;

namespace ElectroHuila.Application.DTOs.Setup;

/// <summary>
/// Data transfer object for configuring initial system data during setup.
/// Used to bootstrap the system with initial branches, appointment types, and clients.
/// </summary>
public class ConfigureInitialDataDto
{
    /// <summary>
    /// The list of branches to create during initial setup.
    /// </summary>
    public List<BranchDto>? Branches { get; set; }

    /// <summary>
    /// The list of appointment types to create during initial setup.
    /// </summary>
    public List<AppointmentTypeDto>? AppointmentTypes { get; set; }

    /// <summary>
    /// The list of clients to create during initial setup.
    /// </summary>
    public List<ClientDto>? Clients { get; set; }
}
