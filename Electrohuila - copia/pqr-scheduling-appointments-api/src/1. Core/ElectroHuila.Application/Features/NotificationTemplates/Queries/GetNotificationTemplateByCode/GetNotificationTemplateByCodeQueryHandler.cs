using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;

namespace ElectroHuila.Application.Features.NotificationTemplates.Queries.GetNotificationTemplateByCode;

/// <summary>
/// Manejador de query para obtener una plantilla de notificación por código
/// </summary>
public class GetNotificationTemplateByCodeQueryHandler : IRequestHandler<GetNotificationTemplateByCodeQuery, Result<NotificationTemplateDto>>
{
    private readonly INotificationTemplateRepository _repository;
    private readonly IMapper _mapper;

    public GetNotificationTemplateByCodeQueryHandler(
        INotificationTemplateRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<NotificationTemplateDto>> Handle(GetNotificationTemplateByCodeQuery request, CancellationToken cancellationToken)
    {
        var template = await _repository.GetByCodeAsync(request.TemplateCode, cancellationToken);
        if (template == null)
        {
            return Result.Failure<NotificationTemplateDto>($"Plantilla con código '{request.TemplateCode}' no encontrada");
        }

        var templateDto = _mapper.Map<NotificationTemplateDto>(template);
        return Result.Success(templateDto);
    }
}
