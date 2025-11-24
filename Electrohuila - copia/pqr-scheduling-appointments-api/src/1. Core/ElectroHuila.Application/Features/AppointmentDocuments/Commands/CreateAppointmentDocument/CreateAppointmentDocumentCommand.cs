using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentDocuments.Commands.CreateAppointmentDocument;

/// <summary>
/// Comando para crear un documento adjunto a una cita
/// </summary>
public record CreateAppointmentDocumentCommand(CreateAppointmentDocumentDto Dto) : IRequest<Result<AppointmentDocumentDto>>;
