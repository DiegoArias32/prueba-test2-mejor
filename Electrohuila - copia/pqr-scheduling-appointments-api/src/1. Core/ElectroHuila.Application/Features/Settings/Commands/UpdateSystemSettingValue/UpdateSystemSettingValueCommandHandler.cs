using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Commands.UpdateSystemSettingValue;

/// <summary>
/// Handler para actualizar el valor de una configuración del sistema
/// </summary>
public class UpdateSystemSettingValueCommandHandler : IRequestHandler<UpdateSystemSettingValueCommand, Result<SystemSettingDto>>
{
    private readonly ISystemSettingRepository _repository;
    private readonly IMapper _mapper;

    public UpdateSystemSettingValueCommandHandler(ISystemSettingRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SystemSettingDto>> Handle(UpdateSystemSettingValueCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _repository.UpdateValueAsync(
                request.SettingDto.SettingKey,
                request.SettingDto.SettingValue,
                cancellationToken
            );

            if (!success)
            {
                return Result.Failure<SystemSettingDto>($"System setting with key '{request.SettingDto.SettingKey}' not found");
            }

            var setting = await _repository.GetByKeyAsync(request.SettingDto.SettingKey, cancellationToken);
            var settingDto = _mapper.Map<SystemSettingDto>(setting);

            return Result.Success(settingDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<SystemSettingDto>($"Error updating system setting value: {ex.Message}");
        }
    }
}
