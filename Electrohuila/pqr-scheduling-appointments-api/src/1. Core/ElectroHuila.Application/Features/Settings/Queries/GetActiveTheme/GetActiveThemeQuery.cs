using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Queries.GetActiveTheme;

/// <summary>
/// Query para obtener el tema activo de la aplicación
/// </summary>
public record GetActiveThemeQuery() : IRequest<Result<ThemeSettingsDto>>;
