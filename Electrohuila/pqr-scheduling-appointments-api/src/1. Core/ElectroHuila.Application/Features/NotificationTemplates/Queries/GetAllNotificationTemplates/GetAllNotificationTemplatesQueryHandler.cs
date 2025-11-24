using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;

namespace ElectroHuila.Application.Features.NotificationTemplates.Queries.GetAllNotificationTemplates;

/// <summary>
/// Manejador de query para obtener todas las plantillas de notificación
/// </summary>
public class GetAllNotificationTemplatesQueryHandler : IRequestHandler<GetAllNotificationTemplatesQuery, Result<IEnumerable<NotificationTemplateDto>>>
{
    private readonly INotificationTemplateRepository _repository;
    private readonly IMapper _mapper;

    public GetAllNotificationTemplatesQueryHandler(
        INotificationTemplateRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<NotificationTemplateDto>>> Handle(GetAllNotificationTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _repository.GetAllAsync();
        var templateDtos = _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);

        return Result.Success(templateDtos);
    }
}
