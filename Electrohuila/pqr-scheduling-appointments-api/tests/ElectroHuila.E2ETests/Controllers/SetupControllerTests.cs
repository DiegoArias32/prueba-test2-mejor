using ElectroHuila.E2ETests.Base;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace ElectroHuila.E2ETests.Controllers;

/// <summary>
/// Pruebas E2E para el controlador de configuración del sistema.
/// Verifica que los endpoints básicos de monitoreo y estado funcionen correctamente.
/// </summary>
public class SetupControllerTests : E2ETestBase
{
    /// <summary>
    /// Inicializa una nueva instancia de las pruebas del controlador de configuración.
    /// </summary>
    /// <param name="factory">Fábrica de aplicación web para crear el cliente de pruebas.</param>
    public SetupControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    /// <summary>
    /// Verifica que el endpoint de salud devuelva el estado correcto del sistema.
    /// </summary>
    [Fact]
    public async Task Health_Should_Return_Healthy_Status()
    {
        // Act
        var response = await Client.GetAsync("/api/v1/setup/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }

    /// <summary>
    /// Verifica que el endpoint de ping responda correctamente para monitoreo.
    /// </summary>
    [Fact]
    public async Task Ping_Should_Return_Pong()
    {
        // Act
        var response = await Client.GetAsync("/api/v1/setup/ping");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("pong");
    }

    /// <summary>
    /// Verifica que el endpoint de información devuelva los datos de la aplicación.
    /// </summary>
    [Fact]
    public async Task Info_Should_Return_Application_Information()
    {
        // Act
        var response = await Client.GetAsync("/api/v1/setup/info");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("ElectroHuila");
        content.Should().Contain("version");
    }
}