using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Commands.UpdateThemeSettings;

/// <summary>
/// Handler para actualizar la configuración del tema
/// </summary>
public class UpdateThemeSettingsCommandHandler : IRequestHandler<UpdateThemeSettingsCommand, Result<ThemeSettingsDto>>
{
    private readonly IThemeSettingsRepository _themeRepository;
    private readonly IMapper _mapper;

    public UpdateThemeSettingsCommandHandler(IThemeSettingsRepository themeRepository, IMapper mapper)
    {
        _themeRepository = themeRepository;
        _mapper = mapper;
    }

    public async Task<Result<ThemeSettingsDto>> Handle(UpdateThemeSettingsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var theme = await _themeRepository.GetByIdAsync(request.Id);
            if (theme == null)
            {
                return Result.Failure<ThemeSettingsDto>("Theme not found");
            }

            var dto = request.ThemeDto;

            // Actualizar detalles del tema
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                theme.UpdateDetails(dto.Name, dto.Description);
            }

            // Actualizar colores principales si se proporcionan
            if (!string.IsNullOrWhiteSpace(dto.ColorPrimary) ||
                !string.IsNullOrWhiteSpace(dto.ColorSecondary) ||
                !string.IsNullOrWhiteSpace(dto.ColorAccent))
            {
                theme.UpdateMainColors(
                    dto.ColorPrimary ?? theme.ColorPrimary,
                    dto.ColorSecondary ?? theme.ColorSecondary,
                    dto.ColorAccent ?? theme.ColorAccent,
                    dto.ColorIntermediate
                );
            }

            // Actualizar colores de estado si se proporcionan
            if (!string.IsNullOrWhiteSpace(dto.ColorSuccess) ||
                !string.IsNullOrWhiteSpace(dto.ColorError) ||
                !string.IsNullOrWhiteSpace(dto.ColorWarning) ||
                !string.IsNullOrWhiteSpace(dto.ColorInfo))
            {
                theme.UpdateStatusColors(
                    dto.ColorSuccess,
                    dto.ColorError,
                    dto.ColorWarning,
                    dto.ColorInfo
                );
            }

            // Actualizar colores de fondo si se proporcionan
            if (!string.IsNullOrWhiteSpace(dto.BackgroundPrimary) ||
                !string.IsNullOrWhiteSpace(dto.BackgroundSecondary))
            {
                theme.UpdateBackgroundColors(
                    dto.BackgroundPrimary ?? theme.BackgroundPrimary,
                    dto.BackgroundSecondary ?? theme.BackgroundSecondary
                );
            }

            // Actualizar colores de texto si se proporcionan
            if (!string.IsNullOrWhiteSpace(dto.TextPrimary) ||
                !string.IsNullOrWhiteSpace(dto.TextSecondary))
            {
                theme.UpdateTextColors(
                    dto.TextPrimary ?? theme.TextPrimary,
                    dto.TextSecondary ?? theme.TextSecondary
                );
            }

            // Actualizar colores de scrollbar si se proporcionan
            if (!string.IsNullOrWhiteSpace(dto.ScrollbarGradientStart) ||
                !string.IsNullOrWhiteSpace(dto.ScrollbarGradientEnd) ||
                !string.IsNullOrWhiteSpace(dto.ScrollbarHoverStart) ||
                !string.IsNullOrWhiteSpace(dto.ScrollbarHoverEnd))
            {
                theme.UpdateScrollbarColors(
                    dto.ScrollbarGradientStart ?? theme.ScrollbarGradientStart,
                    dto.ScrollbarGradientEnd ?? theme.ScrollbarGradientEnd,
                    dto.ScrollbarHoverStart ?? theme.ScrollbarHoverStart,
                    dto.ScrollbarHoverEnd ?? theme.ScrollbarHoverEnd
                );
            }

            await _themeRepository.UpdateAsync(theme);

            var themeDto = _mapper.Map<ThemeSettingsDto>(theme);
            return Result.Success(themeDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<ThemeSettingsDto>($"Error updating theme settings: {ex.Message}");
        }
    }
}
