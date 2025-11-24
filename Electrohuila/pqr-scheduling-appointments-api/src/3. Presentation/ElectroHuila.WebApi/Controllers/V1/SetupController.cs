using ElectroHuila.Application.DTOs.Setup;
using ElectroHuila.Application.Features.Setup.Commands.InitializeData;
using ElectroHuila.Application.Features.Setup.Commands.ConfigureSchedule;
using ElectroHuila.Application.Features.Setup.Commands.BulkConfigureSchedule;
using ElectroHuila.Application.Features.Setup.Commands.ConfigureInitialData;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para configuración inicial y utilitarios del sistema.
/// Proporciona endpoints para inicialización de datos, health checks y configuración.
/// Todos los endpoints son públicos (no requieren autenticación).
/// </summary>
[AllowAnonymous]
public class SetupController : ApiController
{
    /// <summary>
    /// Endpoint de health check para verificar el estado del servicio.
    /// Usado por herramientas de monitoreo y balanceadores de carga.
    /// </summary>
    /// <returns>Estado del servicio con información básica</returns>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            service = "ElectroHuila API",
            version = "1.0.0"
        });
    }

    /// <summary>
    /// Endpoint simple de ping para verificar conectividad básica.
    /// Respuesta rápida para tests de conectividad.
    /// </summary>
    /// <returns>Mensaje pong con timestamp</returns>
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new { message = "pong", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Proporciona información detallada sobre la aplicación.
    /// Incluye versión, entorno y características principales.
    /// </summary>
    /// <returns>Información completa del sistema</returns>
    [HttpGet("info")]
    public IActionResult Info()
    {
        return Ok(new
        {
            application = "ElectroHuila - Sistema de Agendamiento de Citas",
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            timestamp = DateTime.UtcNow,
            features = new[]
            {
                "Agendamiento de citas",
                "Gestión de usuarios y roles",
                "Gestión de sucursales",
                "Solicitudes de nuevas acometidas",
                "Proyectos nuevos"
            }
        });
    }

    /// <summary>
    /// Inicializa los datos básicos del sistema.
    /// Crea datos semilla necesarios para el funcionamiento inicial.
    /// Usado durante la configuración inicial del sistema.
    /// </summary>
    /// <returns>Resultado de la inicialización de datos</returns>
    [HttpPost("init-data")]
    public async Task<IActionResult> InitializeData()
    {
        var command = new InitializeDataCommand();
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Configura un horario de atención específico.
    /// Establece horarios disponibles para agendamiento en sucursales.
    /// </summary>
    /// <param name="dto">Configuración del horario (sucursal, días, horas)</param>
    /// <returns>Resultado de la configuración del horario</returns>
    [HttpPost("configure-schedule")]
    public async Task<IActionResult> ConfigureSchedule([FromBody] ConfigureScheduleDto dto)
    {
        var command = new ConfigureScheduleCommand(dto);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Configura múltiples horarios de atención en una sola operación.
    /// Permite configuración masiva de horarios para múltiples sucursales.
    /// </summary>
    /// <param name="configurations">Lista de configuraciones de horarios</param>
    /// <returns>Resultado de la configuración masiva</returns>
    [HttpPost("bulk-configure-schedule")]
    public async Task<IActionResult> BulkConfigureSchedule([FromBody] List<ConfigureScheduleDto> configurations)
    {
        var command = new BulkConfigureScheduleCommand(configurations);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Configura datos iniciales personalizados del sistema.
    /// Permite establecer configuraciones específicas durante la instalación.
    /// </summary>
    /// <param name="dto">Datos de configuración inicial personalizada</param>
    /// <returns>Resultado de la configuración inicial</returns>
    [HttpPost("configure-initial-data")]
    public async Task<IActionResult> ConfigureInitialData([FromBody] ConfigureInitialDataDto dto)
    {
        var command = new ConfigureInitialDataCommand(dto);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}