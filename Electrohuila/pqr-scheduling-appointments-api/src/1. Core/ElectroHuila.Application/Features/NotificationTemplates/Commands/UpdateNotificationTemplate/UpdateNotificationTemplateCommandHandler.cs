using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;

namespace ElectroHuila.Application.Features.NotificationTemplates.Commands.UpdateNotificationTemplate;

/// <summary>
/// Manejador del comando para actualizar una plantilla de notificación
/// </summary>
public class UpdateNotificationTemplateCommandHandler : IRequestHandler<UpdateNotificationTemplateCommand, Result<NotificationTemplateDto>>
{
    private readonly INotificationTemplateRepository _repository;
    private readonly IMapper _mapper;

    public UpdateNotificationTemplateCommandHandler(
        INotificationTemplateRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<NotificationTemplateDto>> Handle(UpdateNotificationTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(request.Dto.Id);
        if (template == null)
        {
            return Result.Failure<NotificationTemplateDto>($"Plantilla con ID {request.Dto.Id} no encontrada");
        }

        // Actualizar plantilla
        template.UpdateTemplate(request.Dto.Subject, request.Dto.BodyTemplate);
        template.UpdateDetails(request.Dto.TemplateName);

        if (!string.IsNullOrEmpty(request.Dto.Placeholders))
        {
            template.UpdatePlaceholders(request.Dto.Placeholders);
        }

        await _repository.UpdateAsync(template);

        var dto = _mapper.Map<NotificationTemplateDto>(template);
        return Result.Success(dto);
    }
}
