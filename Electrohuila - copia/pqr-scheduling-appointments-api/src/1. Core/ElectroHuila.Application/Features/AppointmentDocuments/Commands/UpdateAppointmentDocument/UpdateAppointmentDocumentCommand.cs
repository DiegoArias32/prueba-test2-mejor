using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentDocuments.Commands.UpdateAppointmentDocument;

/// <summary>
/// Comando para actualizar un documento adjunto
/// </summary>
public record UpdateAppointmentDocumentCommand(UpdateAppointmentDocumentDto Dto) : IRequest<Result<AppointmentDocumentDto>>;
