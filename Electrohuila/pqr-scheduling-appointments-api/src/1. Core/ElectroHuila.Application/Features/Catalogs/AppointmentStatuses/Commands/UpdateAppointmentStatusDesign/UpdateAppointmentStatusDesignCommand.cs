using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Catalogs.AppointmentStatuses.Commands.UpdateAppointmentStatusDesign;

/// <summary>
/// Command para actualizar el diseño (colores e icono) de un estado de cita
/// </summary>
public record UpdateAppointmentStatusDesignCommand(
    int Id,
    string? ColorPrimary,
    string? ColorSecondary,
    string? ColorText,
    string? IconName
) : IRequest<Result<AppointmentStatusDto>>;
