using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Commands.UpdateSystemSetting;

/// <summary>
/// Command para actualizar una configuración del sistema
/// </summary>
public record UpdateSystemSettingCommand(UpdateSystemSettingDto SettingDto) : IRequest<Result<SystemSettingDto>>;
