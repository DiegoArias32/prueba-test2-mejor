using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Commands.UpdateThemeSettings;

/// <summary>
/// Command para actualizar la configuración del tema
/// </summary>
public record UpdateThemeSettingsCommand(int Id, UpdateThemeSettingsDto ThemeDto) : IRequest<Result<ThemeSettingsDto>>;
