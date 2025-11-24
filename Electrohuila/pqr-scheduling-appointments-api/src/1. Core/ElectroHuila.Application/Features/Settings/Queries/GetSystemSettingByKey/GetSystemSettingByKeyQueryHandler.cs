using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Queries.GetSystemSettingByKey;

/// <summary>
/// Handler para obtener una configuración del sistema por su clave
/// </summary>
public class GetSystemSettingByKeyQueryHandler : IRequestHandler<GetSystemSettingByKeyQuery, Result<SystemSettingDto>>
{
    private readonly ISystemSettingRepository _repository;
    private readonly IMapper _mapper;

    public GetSystemSettingByKeyQueryHandler(ISystemSettingRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SystemSettingDto>> Handle(GetSystemSettingByKeyQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var setting = await _repository.GetByKeyAsync(request.SettingKey, cancellationToken);
            if (setting == null)
            {
                return Result.Failure<SystemSettingDto>($"System setting with key '{request.SettingKey}' not found");
            }

            var settingDto = _mapper.Map<SystemSettingDto>(setting);
            return Result.Success(settingDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<SystemSettingDto>($"Error retrieving system setting: {ex.Message}");
        }
    }
}
