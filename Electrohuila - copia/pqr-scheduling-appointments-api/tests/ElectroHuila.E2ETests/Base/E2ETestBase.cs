using Microsoft.AspNetCore.Mvc.Testing;

namespace ElectroHuila.E2ETests.Base;

/// <summary>
/// Clase base para pruebas end-to-end (E2E) de la aplicación ElectroHuila.
/// Proporciona infraestructura común para pruebas de integración completas,
/// incluyendo configuración de cliente HTTP y autenticación simulada.
/// </summary>
public class E2ETestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;

    /// <summary>
    /// Inicializa una nueva instancia de la clase base para pruebas E2E.
    /// Configura la factory de aplicación web y el cliente HTTP para las pruebas.
    /// </summary>
    /// <param name="factory">Factory de aplicación web para crear instancias de prueba</param>
    public E2ETestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    /// <summary>
    /// Obtiene un token de autenticación simulado para las pruebas.
    /// Implementación temporal que devuelve un token mock.
    /// TODO: Implementar lógica de login real para pruebas de autenticación.
    /// </summary>
    /// <returns>Token JWT simulado para autenticación en pruebas</returns>
    protected async Task<string> GetAuthTokenAsync()
    {
        // Mock authentication - implement actual login logic
        return await Task.FromResult("mock-jwt-token");
    }
}