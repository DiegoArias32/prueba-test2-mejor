using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers;

/// <summary>
/// Controller para verificar el estado de salud de la API y sus despliegues
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public HealthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Obtiene el estado de salud de la API y el estado de todos los entornos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetHealth()
    {
        var currentEnvironment = _configuration["ApplicationSettings:Environment"] ?? "Unknown";

        var healthStatus = new
        {
            service = "ElectroHuila API - PQR Agendamiento de Citas",
            status = "healthy",
            timestamp = DateTime.UtcNow,
            currentEnvironment = currentEnvironment,
            version = _configuration["ApplicationSettings:Version"] ?? "1.0.0",
            environments = new
            {
                dev = new
                {
                    deployed = currentEnvironment.Equals("Development", StringComparison.OrdinalIgnoreCase),
                    branch = "dev",
                    port = 5000,
                    url = "http://localhost:5000",
                    description = "Entorno de desarrollo"
                },
                qa = new
                {
                    deployed = currentEnvironment.Equals("QA", StringComparison.OrdinalIgnoreCase),
                    branch = "qa",
                    port = 5002,
                    url = "http://localhost:5002",
                    description = "Entorno de control de calidad"
                },
                staging = new
                {
                    deployed = currentEnvironment.Equals("Staging", StringComparison.OrdinalIgnoreCase),
                    branch = "staging",
                    port = 5001,
                    url = "http://localhost:5001",
                    description = "Entorno de staging/pre-producción"
                },
                main = new
                {
                    deployed = currentEnvironment.Equals("Production", StringComparison.OrdinalIgnoreCase),
                    branch = "main",
                    port = 80,
                    url = "http://localhost",
                    description = "Entorno de producción"
                }
            }
        };

        return Ok(healthStatus);
    }
}
