using Microsoft.AspNetCore.Mvc.Testing;

namespace ElectroHuila.IntegrationTests.Base;

/// <summary>
/// Clase base para pruebas de integración del sistema ElectroHuila.
/// Proporciona la infraestructura común para probar la aplicación completa.
/// </summary>
public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// Fábrica de aplicación web para crear instancias de prueba.
    /// </summary>
    protected readonly WebApplicationFactory<Program> Factory;
    
    /// <summary>
    /// Cliente HTTP configurado para realizar peticiones a la aplicación de prueba.
    /// </summary>
    protected readonly HttpClient Client;

    /// <summary>
    /// Inicializa una nueva instancia de la clase base para pruebas de integración.
    /// </summary>
    /// <param name="factory">Fábrica de aplicación web para configurar el entorno de pruebas.</param>
    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }
}