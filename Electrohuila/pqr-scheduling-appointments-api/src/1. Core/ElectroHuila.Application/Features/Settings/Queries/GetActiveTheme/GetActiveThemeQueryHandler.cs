using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Domain.Entities.Settings;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Queries.GetActiveTheme;

/// <summary>
/// Handler para obtener el tema activo
/// Si no existe tema en la BD, devuelve el tema por defecto
/// </summary>
public class GetActiveThemeQueryHandler : IRequestHandler<GetActiveThemeQuery, Result<ThemeSettingsDto>>
{
    private readonly IThemeSettingsRepository _themeRepository;
    private readonly IMapper _mapper;

    public GetActiveThemeQueryHandler(IThemeSettingsRepository themeRepository, IMapper mapper)
    {
        _themeRepository = themeRepository;
        _mapper = mapper;
    }

    public async Task<Result<ThemeSettingsDto>> Handle(GetActiveThemeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var theme = await _themeRepository.GetActiveThemeAsync();

            // Si no existe tema en la BD, crear y devolver el tema por defecto
            if (theme == null)
            {
                theme = ThemeSettings.CreateDefault();
                await _themeRepository.AddAsync(theme);
            }

            var themeDto = _mapper.Map<ThemeSettingsDto>(theme);
            return Result.Success(themeDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<ThemeSettingsDto>($"Error retrieving active theme: {ex.Message}");
        }
    }
}
