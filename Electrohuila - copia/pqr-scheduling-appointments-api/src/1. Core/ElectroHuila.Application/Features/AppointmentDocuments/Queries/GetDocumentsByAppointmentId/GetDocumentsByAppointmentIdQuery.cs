using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentDocuments.Queries.GetDocumentsByAppointmentId;

/// <summary>
/// Query para obtener todos los documentos de una cita
/// </summary>
public record GetDocumentsByAppointmentIdQuery(int AppointmentId) : IRequest<Result<IEnumerable<AppointmentDocumentDto>>>;
