using ElectroHuila.WebApi.Middleware;

namespace ElectroHuila.WebApi.Extensions;

/// <summary>
/// Extensiones para configurar el pipeline de middleware de la aplicación web.
/// Centraliza la configuración de middlewares en el orden correcto.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configura el pipeline completo de middleware para la Web API.
    /// Establece el orden correcto de middleware para funcionamiento óptimo.
    /// </summary>
    /// <param name="app">Builder de la aplicación</param>
    /// <param name="env">Información del entorno de ejecución</param>
    /// <returns>Builder configurado con todos los middlewares</returns>
    public static IApplicationBuilder UseWebApiMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configuración específica para desarrollo
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElectroHuila API V1");
            });
        }

        // Middleware personalizado para manejo de excepciones
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        // Middleware personalizado para logging de requests
        app.UseMiddleware<RequestLoggingMiddleware>();

        // Middleware de ASP.NET Core
        app.UseHttpsRedirection();
        app.UseCors("AllowSpecificOrigins");

        // Middleware de autenticación y autorización (orden importante)
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}