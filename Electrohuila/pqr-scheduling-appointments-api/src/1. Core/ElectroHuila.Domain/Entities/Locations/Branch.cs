using ElectroHuila.Domain.Entities.Common;
using ElectroHuila.Domain.Entities.Appointments;

namespace ElectroHuila.Domain.Entities.Locations;

/// <summary>
/// Representa una sucursal de la empresa
/// </summary>
public class Branch : BaseEntity
{
    /// <summary>
    /// Nombre de la sucursal
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Código único de la sucursal
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Dirección física de la sucursal
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Número de teléfono de la sucursal
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Ciudad donde se ubica la sucursal
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Departamento donde se ubica la sucursal
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Indica si es la sucursal principal
    /// </summary>
    public bool IsMain { get; set; } = false;

    /// <summary>
    /// Color principal para identificar la sede en el frontend
    /// </summary>
    public string? ColorPrimary { get; set; }

    /// <summary>
    /// Colección de citas programadas en esta sucursal
    /// </summary>
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    /// <summary>
    /// Actualiza el color de la sucursal
    /// </summary>
    public void UpdateColor(string? colorPrimary)
    {
        ColorPrimary = colorPrimary;
    }
}