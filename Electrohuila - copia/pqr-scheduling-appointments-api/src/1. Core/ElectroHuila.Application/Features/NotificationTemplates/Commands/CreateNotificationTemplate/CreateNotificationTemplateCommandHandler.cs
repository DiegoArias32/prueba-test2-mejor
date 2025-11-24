using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Notifications;
using ElectroHuila.Domain.Entities.Notifications;
using MediatR;

namespace ElectroHuila.Application.Features.NotificationTemplates.Commands.CreateNotificationTemplate;

/// <summary>
/// Manejador del comando para crear una plantilla de notificación
/// </summary>
public class CreateNotificationTemplateCommandHandler : IRequestHandler<CreateNotificationTemplateCommand, Result<NotificationTemplateDto>>
{
    private readonly INotificationTemplateRepository _repository;
    private readonly IMapper _mapper;

    public CreateNotificationTemplateCommandHandler(
        INotificationTemplateRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<NotificationTemplateDto>> Handle(CreateNotificationTemplateCommand request, CancellationToken cancellationToken)
    {
        // Verificar si ya existe una plantilla con ese código
        var existingTemplate = await _repository.GetByCodeAsync(request.Dto.TemplateCode, cancellationToken);
        if (existingTemplate != null)
        {
            return Result.Failure<NotificationTemplateDto>($"Ya existe una plantilla con el código '{request.Dto.TemplateCode}'");
        }

        // Crear la plantilla
        var template = NotificationTemplate.Create(
            request.Dto.TemplateCode,
            request.Dto.TemplateName,
            request.Dto.BodyTemplate,
            request.Dto.TemplateType,
            request.Dto.Subject,
            request.Dto.Placeholders
        );

        await _repository.AddAsync(template);

        var dto = _mapper.Map<NotificationTemplateDto>(template);
        return Result.Success(dto);
    }
}
