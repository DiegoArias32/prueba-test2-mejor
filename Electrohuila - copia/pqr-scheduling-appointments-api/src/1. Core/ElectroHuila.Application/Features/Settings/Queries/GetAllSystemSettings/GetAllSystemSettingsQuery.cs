using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Queries.GetAllSystemSettings;

/// <summary>
/// Query para obtener todas las configuraciones del sistema
/// </summary>
public record GetAllSystemSettingsQuery : IRequest<Result<IEnumerable<SystemSettingDto>>>;
