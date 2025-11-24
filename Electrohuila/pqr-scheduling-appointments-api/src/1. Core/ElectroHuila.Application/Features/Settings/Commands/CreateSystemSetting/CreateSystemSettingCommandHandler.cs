using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Domain.Entities.Settings;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Commands.CreateSystemSetting;

/// <summary>
/// Handler para crear una nueva configuración del sistema
/// </summary>
public class CreateSystemSettingCommandHandler : IRequestHandler<CreateSystemSettingCommand, Result<SystemSettingDto>>
{
    private readonly ISystemSettingRepository _repository;
    private readonly IMapper _mapper;

    public CreateSystemSettingCommandHandler(ISystemSettingRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SystemSettingDto>> Handle(CreateSystemSettingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar si ya existe una configuración con la misma clave
            var exists = await _repository.ExistsByKeyAsync(request.SettingDto.SettingKey, cancellationToken);
            if (exists)
            {
                return Result.Failure<SystemSettingDto>($"A system setting with key '{request.SettingDto.SettingKey}' already exists");
            }

            var setting = SystemSetting.Create(
                request.SettingDto.SettingKey,
                request.SettingDto.SettingValue,
                request.SettingDto.SettingType,
                request.SettingDto.Description,
                request.SettingDto.IsEncrypted
            );

            await _repository.AddAsync(setting);
            var settingDto = _mapper.Map<SystemSettingDto>(setting);

            return Result.Success(settingDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<SystemSettingDto>($"Error creating system setting: {ex.Message}");
        }
    }
}
