using ElectroHuila.Application.Common.Interfaces.Services.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Infrastructure.Persistence.Seeds;

/// <summary>
/// Proporciona métodos de extensión para la inicialización automática de la base de datos
/// durante el startup de la aplicación ASP.NET Core.
/// </summary>
/// <remarks>
/// Esta clase contiene extensiones que facilitan la integración del proceso de seeding
/// en el pipeline de inicialización de la aplicación, combinando migraciones y
/// datos iniciales en una sola operación.
/// 
/// INTEGRACIÓN CON STARTUP:
/// Diseñado para ser llamado desde Program.cs o Startup.cs durante
/// la configuración del pipeline de la aplicación.
/// 
/// PATRÓN UTILIZADO:
/// Extension methods sobre IApplicationBuilder para seguir las
/// convenciones estándar de ASP.NET Core.
/// </remarks>
public static class SeedExtensions
{
    /// <summary>
    /// Ejecuta las migraciones pendientes y inicializa los datos maestros de la base de datos.
    /// </summary>
    /// <param name="app">La instancia de IApplicationBuilder de la aplicación.</param>
    /// <returns>
    /// La misma instancia de IApplicationBuilder para permitir method chaining.
    /// </returns>
    /// <remarks>
    /// PROCESO EJECUTADO:
    /// 1. Crea un scope de servicios para acceso a dependencias
    /// 2. Resuelve servicios requeridos (DbContext, PasswordHasher, Logger)
    /// 3. Aplica migraciones pendientes automáticamente
    /// 4. Ejecuta el proceso completo de seeding de datos
    /// 5. Maneja errores y logging apropiadamente
    /// 
    /// SERVICIOS REQUERIDOS:
    /// - ApplicationDbContext: Para acceso a la base de datos
    /// - IPasswordHasher: Para hash de contraseñas del usuario admin
    /// - ILogger<DatabaseSeeder>: Para logging del proceso de seeding
    /// 
    /// OPERACIONES AUTOMÁTICAS:
    /// - MigrateAsync(): Aplica todas las migraciones pendientes
    /// - SeedAsync(): Ejecuta la inicialización de datos maestros
    /// 
    /// USO TÍPICO EN PROGRAM.CS:
    /// ```csharp
    /// var app = builder.Build();
    /// await app.SeedDatabaseAsync();
    /// app.Run();
    /// ```
    /// 
    /// CONSIDERACIONES DE PRODUCCIÓN:
    /// - Seguro para ejecutar múltiples veces (idempotente)
    /// - Aplica migraciones automáticamente
    /// - Logging detallado para troubleshooting
    /// - Manejo robusto de errores con propagación
    /// 
    /// RENDIMIENTO:
    /// Ejecutado solo durante startup, no afecta rendimiento en runtime.
    /// Para bases de datos grandes, considerar estrategias de seeding offline.
    /// 
    /// SCOPE MANAGEMENT:
    /// Utiliza using var scope para garantizar disposición apropiada
    /// de recursos, especialmente importante para DbContext.
    /// </remarks>
    public static async Task<IApplicationBuilder> SeedDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var passwordHasher = services.GetRequiredService<IPasswordHasher>();
            var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();

            // Apply pending migrations
            await context.Database.MigrateAsync();

            // Seed data
            var seeder = new DatabaseSeeder(context, passwordHasher, logger);
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeding");
            logger.LogError(ex, "An error occurred while migrating or seeding the database");
            throw;
        }

        return app;
    }
}