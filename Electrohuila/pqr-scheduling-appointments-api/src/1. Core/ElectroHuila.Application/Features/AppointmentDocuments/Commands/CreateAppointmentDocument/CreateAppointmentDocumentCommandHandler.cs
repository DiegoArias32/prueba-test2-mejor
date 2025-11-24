using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Domain.Entities.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentDocuments.Commands.CreateAppointmentDocument;

/// <summary>
/// Manejador del comando para crear un documento adjunto
/// </summary>
public class CreateAppointmentDocumentCommandHandler : IRequestHandler<CreateAppointmentDocumentCommand, Result<AppointmentDocumentDto>>
{
    private readonly IAppointmentDocumentRepository _documentRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public CreateAppointmentDocumentCommandHandler(
        IAppointmentDocumentRepository documentRepository,
        IAppointmentRepository appointmentRepository,
        IMapper mapper)
    {
        _documentRepository = documentRepository;
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentDocumentDto>> Handle(CreateAppointmentDocumentCommand request, CancellationToken cancellationToken)
    {
        // Verificar que la cita existe
        var appointment = await _appointmentRepository.GetByIdAsync(request.Dto.AppointmentId);
        if (appointment == null)
        {
            return Result.Failure<AppointmentDocumentDto>($"Cita con ID {request.Dto.AppointmentId} no encontrada");
        }

        // Crear el documento
        var document = AppointmentDocument.Create(
            request.Dto.AppointmentId,
            request.Dto.DocumentName,
            request.Dto.FilePath,
            request.Dto.DocumentType,
            request.Dto.FileSize,
            request.Dto.UploadedBy,
            request.Dto.Description
        );

        await _documentRepository.AddAsync(document);

        var dto = _mapper.Map<AppointmentDocumentDto>(document);
        return Result.Success(dto);
    }
}
