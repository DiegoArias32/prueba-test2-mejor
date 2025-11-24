using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Commands.UpdateSystemSettingValue;

/// <summary>
/// Command para actualizar el valor de una configuración del sistema
/// </summary>
public record UpdateSystemSettingValueCommand(UpdateSystemSettingValueDto SettingDto) : IRequest<Result<SystemSettingDto>>;
