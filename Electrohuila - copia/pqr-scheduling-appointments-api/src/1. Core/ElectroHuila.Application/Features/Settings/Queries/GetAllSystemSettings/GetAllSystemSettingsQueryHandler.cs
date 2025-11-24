using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Queries.GetAllSystemSettings;

/// <summary>
/// Handler para obtener todas las configuraciones del sistema
/// </summary>
public class GetAllSystemSettingsQueryHandler : IRequestHandler<GetAllSystemSettingsQuery, Result<IEnumerable<SystemSettingDto>>>
{
    private readonly ISystemSettingRepository _repository;
    private readonly IMapper _mapper;

    public GetAllSystemSettingsQueryHandler(ISystemSettingRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<SystemSettingDto>>> Handle(GetAllSystemSettingsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var settings = await _repository.GetAllAsync();
            var settingDtos = _mapper.Map<IEnumerable<SystemSettingDto>>(settings);

            return Result.Success(settingDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<SystemSettingDto>>($"Error retrieving system settings: {ex.Message}");
        }
    }
}
