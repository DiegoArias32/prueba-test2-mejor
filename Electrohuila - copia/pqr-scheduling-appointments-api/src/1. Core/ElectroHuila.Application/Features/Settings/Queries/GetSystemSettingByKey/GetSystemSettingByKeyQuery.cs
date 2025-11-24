using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Settings.Queries.GetSystemSettingByKey;

/// <summary>
/// Query para obtener una configuración del sistema por su clave
/// </summary>
public record GetSystemSettingByKeyQuery(string SettingKey) : IRequest<Result<SystemSettingDto>>;
