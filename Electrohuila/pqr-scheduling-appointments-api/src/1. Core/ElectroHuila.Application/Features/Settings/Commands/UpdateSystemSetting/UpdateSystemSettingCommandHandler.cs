using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Commands.UpdateSystemSetting;

/// <summary>
/// Handler para actualizar una configuración del sistema
/// </summary>
public class UpdateSystemSettingCommandHandler : IRequestHandler<UpdateSystemSettingCommand, Result<SystemSettingDto>>
{
    private readonly ISystemSettingRepository _systemSettingRepository;
    private readonly IMapper _mapper;

    public UpdateSystemSettingCommandHandler(ISystemSettingRepository systemSettingRepository, IMapper mapper)
    {
        _systemSettingRepository = systemSettingRepository;
        _mapper = mapper;
    }

    public async Task<Result<SystemSettingDto>> Handle(UpdateSystemSettingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.SettingDto;

            // Obtener la configuración existente
            var systemSetting = await _systemSettingRepository.GetByIdAsync(dto.Id);
            if (systemSetting == null)
            {
                return Result.Failure<SystemSettingDto>("System setting not found");
            }

            // Actualizar valor si se proporciona
            if (dto.SettingValue != null)
            {
                systemSetting.UpdateValue(dto.SettingValue);
            }

            // Actualizar descripción si se proporciona
            if (dto.Description != null)
            {
                systemSetting.UpdateDescription(dto.Description);
            }

            // Guardar cambios
            await _systemSettingRepository.UpdateAsync(systemSetting);

            // Mapear a DTO y retornar
            var settingDto = _mapper.Map<SystemSettingDto>(systemSetting);
            return Result.Success(settingDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<SystemSettingDto>($"Error updating system setting: {ex.Message}");
        }
    }
}
