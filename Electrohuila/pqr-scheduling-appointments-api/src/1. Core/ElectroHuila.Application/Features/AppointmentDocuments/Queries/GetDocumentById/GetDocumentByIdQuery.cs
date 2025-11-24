using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentDocuments.Queries.GetDocumentById;

/// <summary>
/// Query para obtener un documento por ID
/// </summary>
public record GetDocumentByIdQuery(int Id) : IRequest<Result<AppointmentDocumentDto>>;
