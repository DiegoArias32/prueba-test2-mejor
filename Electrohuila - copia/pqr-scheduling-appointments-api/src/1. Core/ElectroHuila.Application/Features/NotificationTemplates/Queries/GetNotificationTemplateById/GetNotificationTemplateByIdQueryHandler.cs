using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;

namespace ElectroHuila.Application.Features.NotificationTemplates.Queries.GetNotificationTemplateById;

/// <summary>
/// Manejador de query para obtener una plantilla de notificación por ID
/// </summary>
public class GetNotificationTemplateByIdQueryHandler : IRequestHandler<GetNotificationTemplateByIdQuery, Result<NotificationTemplateDto>>
{
    private readonly INotificationTemplateRepository _repository;
    private readonly IMapper _mapper;

    public GetNotificationTemplateByIdQueryHandler(
        INotificationTemplateRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<NotificationTemplateDto>> Handle(GetNotificationTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(request.Id);
        if (template == null)
        {
            return Result.Failure<NotificationTemplateDto>($"Plantilla con ID {request.Id} no encontrada");
        }

        var templateDto = _mapper.Map<NotificationTemplateDto>(template);
        return Result.Success(templateDto);
    }
}
