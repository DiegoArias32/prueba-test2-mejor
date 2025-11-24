using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentDocuments.Commands.DeleteAppointmentDocument;

/// <summary>
/// Comando para eliminar un documento adjunto
/// </summary>
public record DeleteAppointmentDocumentCommand(int Id) : IRequest<Result<bool>>;
