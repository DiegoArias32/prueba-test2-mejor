using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.ScheduleSimpleAppointment;

/// <summary>
/// Comando para agendar una cita simple creando el cliente si no existe.
/// Este comando maneja la creación del cliente y la cita en una sola operación.
/// </summary>
public record ScheduleSimpleAppointmentCommand : IRequest<Result<ScheduleSimpleAppointmentResponse>>
{
    // ========== DATOS DEL CLIENTE ==========

    /// <summary>
    /// Tipo de documento del cliente (CC, TI, CE, RC, etc.)
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
    /// Número de teléfono fijo del cliente
    /// </summary>
    public string Phone { get; init; } = string.Empty;

    /// <summary>
    /// Número de teléfono móvil del cliente
    /// </summary>
    public string Mobile { get; init; } = string.Empty;

    /// <summary>
    /// Correo electrónico del cliente
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Dirección física del cliente
    /// </summary>
    public string Address { get; init; } = string.Empty;

    // ========== DATOS DE LA CITA ==========

    /// <summary>
    /// ID de la sucursal donde se realizará la cita
    /// </summary>
    public int BranchId { get; init; }

    /// <summary>
    /// ID del tipo de cita a agendar
    /// </summary>
    public int AppointmentTypeId { get; init; }

    /// <summary>
    /// Fecha de la cita en formato ISO (YYYY-MM-DD)
    /// </summary>
    public string AppointmentDate { get; init; } = string.Empty;

    /// <summary>
    /// Hora de la cita en formato HH:mm
    /// </summary>
    public string AppointmentTime { get; init; } = string.Empty;

    /// <summary>
    /// Observaciones o notas adicionales sobre la cita
    /// </summary>
    public string Observations { get; init; } = string.Empty;
}
