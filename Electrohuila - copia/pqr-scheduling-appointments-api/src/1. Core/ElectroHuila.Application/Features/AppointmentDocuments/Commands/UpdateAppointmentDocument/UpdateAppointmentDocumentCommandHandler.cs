using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentDocuments.Commands.UpdateAppointmentDocument;

/// <summary>
/// Manejador del comando para actualizar un documento adjunto
/// </summary>
public class UpdateAppointmentDocumentCommandHandler : IRequestHandler<UpdateAppointmentDocumentCommand, Result<AppointmentDocumentDto>>
{
    private readonly IAppointmentDocumentRepository _repository;
    private readonly IMapper _mapper;

    public UpdateAppointmentDocumentCommandHandler(
        IAppointmentDocumentRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentDocumentDto>> Handle(UpdateAppointmentDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _repository.GetByIdAsync(request.Dto.Id);
        if (document == null)
        {
            return Result.Failure<AppointmentDocumentDto>($"Documento con ID {request.Dto.Id} no encontrado");
        }

        // Actualizar descripción
        document.UpdateDescription(request.Dto.Description);

        await _repository.UpdateAsync(document);

        var dto = _mapper.Map<AppointmentDocumentDto>(document);
        return Result.Success(dto);
    }
}
