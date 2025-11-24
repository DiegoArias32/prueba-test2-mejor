using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentDocuments.Queries.GetDocumentById;

/// <summary>
/// Manejador de query para obtener un documento por ID
/// </summary>
public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, Result<AppointmentDocumentDto>>
{
    private readonly IAppointmentDocumentRepository _repository;
    private readonly IMapper _mapper;

    public GetDocumentByIdQueryHandler(
        IAppointmentDocumentRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentDocumentDto>> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await _repository.GetByIdAsync(request.Id);
        if (document == null)
        {
            return Result.Failure<AppointmentDocumentDto>($"Documento con ID {request.Id} no encontrado");
        }

        var documentDto = _mapper.Map<AppointmentDocumentDto>(document);
        return Result.Success(documentDto);
    }
}
