using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Commands.CreateSystemSetting;

/// <summary>
/// Command para crear una nueva configuración del sistema
/// </summary>
public record CreateSystemSettingCommand(CreateSystemSettingDto SettingDto) : IRequest<Result<SystemSettingDto>>;
