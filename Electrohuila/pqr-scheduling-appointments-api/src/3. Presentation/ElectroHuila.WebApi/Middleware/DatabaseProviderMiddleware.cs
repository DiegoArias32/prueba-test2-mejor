using ElectroHuila.Infrastructure.Services;

namespace ElectroHuila.WebApi.Middleware;

/// <summary>
/// Middleware para capturar y establecer el proveedor de base de datos
/// desde parámetros de URL o headers HTTP.
/// IMPORTANTE: Solo permite cambio dinámico en el entorno MAIN (Producción).
/// Los demás entornos usan su base de datos fija configurada.
/// </summary>
public class DatabaseProviderMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DatabaseProviderMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public DatabaseProviderMiddleware(
        RequestDelegate next,
        ILogger<DatabaseProviderMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Verificar si estamos en el entorno MAIN (Producción)
        var environment = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";
        var isMainEnvironment = environment.Equals("Production", StringComparison.OrdinalIgnoreCase) 
                             || environment.Equals("Main", StringComparison.OrdinalIgnoreCase);

        // Intentar obtener el proveedor de base de datos de varias fuentes
        string? providerString = null;

        // 1. Desde query parameter 'database'
        if (context.Request.Query.TryGetValue("database", out var dbFromQuery))
        {
            providerString = dbFromQuery.ToString();
            
            if (!isMainEnvironment)
            {
                _logger.LogWarning(
                    "⚠️ Attempt to change database provider ignored. Environment '{Environment}' only allows fixed database. Use MAIN environment for dynamic database switching.",
                    environment);
                providerString = null; // Ignorar el cambio
            }
            else
            {
                _logger.LogInformation("✅ Database provider from query parameter: {Provider} (MAIN environment)", providerString);
            }
        }
        // 2. Desde header 'X-Database-Provider'
        else if (context.Request.Headers.TryGetValue("X-Database-Provider", out var dbFromHeader))
        {
            providerString = dbFromHeader.ToString();
            
            if (!isMainEnvironment)
            {
                _logger.LogWarning(
                    "⚠️ Attempt to change database provider via header ignored. Environment '{Environment}' only allows fixed database. Use MAIN environment for dynamic database switching.",
                    environment);
                providerString = null; // Ignorar el cambio
            }
            else
            {
                _logger.LogInformation("✅ Database provider from header: {Provider} (MAIN environment)", providerString);
            }
        }

        // Si se especificó un proveedor Y estamos en MAIN, almacenarlo en HttpContext.Items
        // El DatabaseProviderService lo leerá cuando se cree el DbContext
        if (!string.IsNullOrEmpty(providerString))
        {
            var provider = ParseDatabaseProvider(providerString);
            if (provider.HasValue)
            {
                // Almacenar ANTES de que se resuelvan los servicios
                context.Items["DatabaseProvider"] = provider.Value.ToString();
                _logger.LogInformation("🔄 Database provider set to {Provider} for this request", provider.Value);
            }
            else
            {
                _logger.LogWarning("❌ Invalid database provider specified: {Provider}", providerString);
            }
        }

        await _next(context);
    }

    private static DatabaseProvider? ParseDatabaseProvider(string providerString)
    {
        return providerString.ToLowerInvariant() switch
        {
            "oracle" => DatabaseProvider.Oracle,
            "sqlserver" => DatabaseProvider.SqlServer,
            "postgresql" => DatabaseProvider.PostgreSQL,
            "mysql" => DatabaseProvider.MySQL,
            _ => null
        };
    }
}

/// <summary>
/// Métodos de extensión para registrar el middleware de proveedor de base de datos.
/// </summary>
public static class DatabaseProviderMiddlewareExtensions
{
    /// <summary>
    /// Agrega el middleware de selección de proveedor de base de datos al pipeline.
    /// </summary>
    public static IApplicationBuilder UseDatabaseProviderSelector(this IApplicationBuilder app)
    {
        return app.UseMiddleware<DatabaseProviderMiddleware>();
    }
}
