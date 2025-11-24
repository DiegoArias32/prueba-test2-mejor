using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentDocuments.Queries.GetDocumentsByAppointmentId;

/// <summary>
/// Manejador de query para obtener documentos por ID de cita
/// </summary>
public class GetDocumentsByAppointmentIdQueryHandler : IRequestHandler<GetDocumentsByAppointmentIdQuery, Result<IEnumerable<AppointmentDocumentDto>>>
{
    private readonly IAppointmentDocumentRepository _repository;
    private readonly IMapper _mapper;

    public GetDocumentsByAppointmentIdQueryHandler(
        IAppointmentDocumentRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AppointmentDocumentDto>>> Handle(GetDocumentsByAppointmentIdQuery request, CancellationToken cancellationToken)
    {
        var documents = await _repository.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);
        var documentDtos = _mapper.Map<IEnumerable<AppointmentDocumentDto>>(documents);

        return Result<IEnumerable<AppointmentDocumentDto>>.Success(documentDtos);
    }
}
