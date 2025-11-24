using Microsoft.AspNetCore.Mvc;
using ElectroHuila.Infrastructure.Services;
using ElectroHuila.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para health checks y diagnóstico de base de datos.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class DatabaseHealthController : ControllerBase
{
    private readonly IDatabaseProviderService _databaseProviderService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<DatabaseHealthController> _logger;
    private readonly IConfiguration _configuration;

    public DatabaseHealthController(
        IDatabaseProviderService databaseProviderService,
        ApplicationDbContext dbContext,
        ILogger<DatabaseHealthController> logger,
        IConfiguration configuration)
    {
        _databaseProviderService = databaseProviderService;
        _dbContext = dbContext;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Verifica la conexión a la base de datos seleccionada y retorna información de diagnóstico completa.
    /// </summary>
    /// <returns>Estado de la conexión, nombre de la base de datos, cantidad de tablas y connection string enmascarado.</returns>
    /// <response code="200">Conexión exitosa. La base de datos está disponible y respondiendo.</response>
    /// <response code="503">No se pudo conectar a la base de datos. Servicio no disponible.</response>
    /// <remarks>
    /// Este endpoint permite verificar la salud de la conexión a la base de datos antes de realizar operaciones críticas.
    /// 
    /// **Ejemplo de uso:**
    /// 
    ///     GET /api/v1/DatabaseHealth/check?database=sqlserver
    /// 
    /// **Respuesta exitosa (200 OK):**
    /// 
    ///     {
    ///       "provider": "SqlServer",
    ///       "isConnected": true,
    ///       "status": "Healthy",
    ///       "message": "Successfully connected to SqlServer database",
    ///       "databaseName": "ElectroHuila_Dev",
    ///       "tableCount": 22,
    ///       "connectionStringPreview": "Server=localhost;Database=***;User Id=***;Password=***",
    ///       "timestamp": "2025-10-20T15:30:00Z"
    ///     }
    /// 
    /// **Respuesta de error (503 Service Unavailable):**
    /// 
    ///     {
    ///       "provider": "PostgreSQL",
    ///       "isConnected": false,
    ///       "status": "Unhealthy",
    ///       "message": "Cannot connect to PostgreSQL database",
    ///       "errorDetails": "NpgsqlException",
    ///       "connectionStringPreview": "Host=localhost;Database=***;Username=***;Password=***",
    ///       "timestamp": "2025-10-20T15:30:00Z"
    ///     }
    /// 
    /// **Tip:** Usa el parámetro `database` para probar diferentes proveedores (oracle, sqlserver, postgresql, mysql).
    /// </remarks>
    [HttpGet("check")]
    [ProducesResponseType(typeof(DatabaseHealthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CheckDatabaseHealth()
    {
        var provider = _databaseProviderService.CurrentProvider;
        var connectionString = _databaseProviderService.GetConnectionString();
        
        _logger.LogInformation("Health check requested for provider: {Provider}", provider);

        var response = new DatabaseHealthResponse
        {
            Provider = provider.ToString(),
            ConnectionStringPreview = MaskConnectionString(connectionString),
            Timestamp = DateTime.UtcNow
        };

        try
        {
            // Intentar conectar y hacer una query simple
            var canConnect = await _dbContext.Database.CanConnectAsync();
            
            if (canConnect)
            {
                // Obtener información adicional de la BD
                var dbName = await GetDatabaseNameAsync();
                var tableCount = await GetTableCountAsync();

                response.IsConnected = true;
                response.Status = "Healthy";
                response.DatabaseName = dbName;
                response.TableCount = tableCount;
                response.Message = $"Successfully connected to {provider} database";

                _logger.LogInformation("Database health check passed for {Provider}", provider);
                
                return Ok(response);
            }
            else
            {
                response.IsConnected = false;
                response.Status = "Unhealthy";
                response.Message = $"Cannot connect to {provider} database";

                _logger.LogWarning("Database health check failed for {Provider}", provider);
                
                return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
            }
        }
        catch (Exception ex)
        {
            response.IsConnected = false;
            response.Status = "Unhealthy";
            response.Message = $"Error connecting to {provider}: {ex.Message}";
            response.ErrorDetails = ex.GetType().Name;

            _logger.LogError(ex, "Database health check error for {Provider}", provider);
            
            return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }
    }

    /// <summary>
    /// Obtiene información detallada sobre el proveedor de base de datos actualmente configurado.
    /// </summary>
    /// <returns>Proveedor actual, lista de proveedores disponibles y connection string enmascarado.</returns>
    /// <response code="200">Información del proveedor recuperada exitosamente.</response>
    /// <remarks>
    /// Este endpoint muestra qué proveedor de base de datos está configurado actualmente y cuáles están disponibles.
    /// 
    /// **Ejemplo de uso:**
    /// 
    ///     GET /api/v1/DatabaseHealth/info?database=oracle
    /// 
    /// **Respuesta (200 OK):**
    /// 
    ///     {
    ///       "currentProvider": "Oracle",
    ///       "connectionStringPreview": "User Id=***;Password=***;Data Source=(DESCRIPTION=...)",
    ///       "availableProviders": ["Oracle", "SqlServer", "PostgreSQL"],
    ///       "message": "Currently configured to use Oracle database"
    ///     }
    /// 
    /// **💡 Tip:** Útil para verificar la configuración antes de ejecutar migraciones o scripts.
    /// </remarks>
    [HttpGet("info")]
    [ProducesResponseType(typeof(DatabaseInfoResponse), StatusCodes.Status200OK)]
    public IActionResult GetDatabaseInfo()
    {
        var provider = _databaseProviderService.CurrentProvider;
        var connectionString = _databaseProviderService.GetConnectionString();

        // Determinar proveedores disponibles según el entorno
        var environment = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";
        var isMainEnvironment = environment.Equals("Production", StringComparison.OrdinalIgnoreCase) 
                             || environment.Equals("Main", StringComparison.OrdinalIgnoreCase);

        string[] availableProviders;
        
        if (isMainEnvironment)
        {
            // Entorno MAIN: Todas las bases de datos disponibles
            availableProviders = new[] { "Oracle", "SqlServer", "PostgreSQL", "MySQL" };
        }
        else
        {
            // Otros entornos: Solo el proveedor actual (fijo)
            availableProviders = new[] { provider.ToString() };
        }

        var response = new DatabaseInfoResponse
        {
            CurrentProvider = provider.ToString(),
            ConnectionStringPreview = MaskConnectionString(connectionString),
            AvailableProviders = availableProviders,
            Message = isMainEnvironment 
                ? $"Currently using {provider} database. You can switch to: {string.Join(", ", availableProviders)}"
                : $"Environment '{environment}' is locked to {provider} database only"
        };

        return Ok(response);
    }

    /// <summary>
    /// Prueba la conectividad con todos los proveedores de base de datos configurados (Oracle, SQL Server, PostgreSQL y MySQL).
    /// </summary>
    /// <returns>Lista con el estado de conexión de cada proveedor disponible.</returns>
    /// <response code="200">Pruebas completadas. Revisa el campo 'isConnected' de cada proveedor para ver los resultados.</response>
    /// <remarks>
    /// Este endpoint realiza una prueba de conexión simultánea a todos los proveedores de base de datos configurados.
    /// Es útil para diagnosticar problemas de conectividad o verificar configuraciones de múltiples entornos.
    /// 
    /// **Ejemplo de uso:**
    /// 
    ///     GET /api/v1/DatabaseHealth/test-all
    /// 
    /// **Respuesta (200 OK):**
    /// 
    ///     {
    ///       "results": [
    ///         {
    ///           "provider": "Oracle",
    ///           "isConfigured": true,
    ///           "isConnected": true,
    ///           "message": "Connection successful",
    ///           "connectionStringPreview": "User Id=***;Password=***;Data Source=..."
    ///         },
    ///         {
    ///           "provider": "SqlServer",
    ///           "isConfigured": true,
    ///           "isConnected": false,
    ///           "message": "Error: A network-related or instance-specific error occurred...",
    ///           "connectionStringPreview": "Server=localhost;Database=***;User Id=***;Password=***"
    ///         },
    ///         {
    ///           "provider": "PostgreSQL",
    ///           "isConfigured": false,
    ///           "isConnected": false,
    ///           "message": "Connection string not configured",
    ///           "connectionStringPreview": null
    ///         }
    ///       ],
    ///       "testedAt": "2025-10-20T15:45:00Z"
    ///     }
    /// 
    /// **💡 Tip:** Ejecuta este endpoint al configurar un nuevo entorno para verificar todas las conexiones de una sola vez.
    /// 
    /// **⚠️ Advertencia:** Este endpoint puede tardar varios segundos si alguna base de datos no está disponible.
    /// </remarks>
    [HttpGet("test-all")]
    [ProducesResponseType(typeof(AllProvidersTestResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> TestAllProviders()
    {
        var results = new List<ProviderTestResult>();

        // Probar Oracle
        results.Add(await TestProvider(DatabaseProvider.Oracle, "OracleConnection"));

        // Probar SQL Server
        results.Add(await TestProvider(DatabaseProvider.SqlServer, "SqlServerConnection"));

        // Probar PostgreSQL
        results.Add(await TestProvider(DatabaseProvider.PostgreSQL, "PostgreSqlConnection"));

        // Probar MySQL
        results.Add(await TestProvider(DatabaseProvider.MySQL, "MySqlConnection"));

        var response = new AllProvidersTestResponse
        {
            Results = results,
            TestedAt = DateTime.UtcNow
        };

        return Ok(response);
    }

    private async Task<ProviderTestResult> TestProvider(DatabaseProvider provider, string connectionKey)
    {
        var result = new ProviderTestResult
        {
            Provider = provider.ToString(),
            IsConfigured = false,
            IsConnected = false
        };

        try
        {
            var connectionString = HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()
                .GetConnectionString(connectionKey);

            if (string.IsNullOrEmpty(connectionString))
            {
                result.Message = "Connection string not configured";
                return result;
            }

            result.IsConfigured = true;
            result.ConnectionStringPreview = MaskConnectionString(connectionString);

            // Crear DbContext temporal con este proveedor
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            switch (provider)
            {
                case DatabaseProvider.Oracle:
                    optionsBuilder.UseOracle(connectionString);
                    break;
                case DatabaseProvider.SqlServer:
                    optionsBuilder.UseSqlServer(connectionString);
                    break;
                case DatabaseProvider.PostgreSQL:
                    optionsBuilder.UseNpgsql(connectionString);
                    break;
                case DatabaseProvider.MySQL:
                    var serverVersion = new MySqlServerVersion(new Version(8, 0, 35));
                    optionsBuilder.UseMySql(connectionString, serverVersion);
                    break;
            }

            using var tempDbContext = new ApplicationDbContext(optionsBuilder.Options);
            result.IsConnected = await tempDbContext.Database.CanConnectAsync();
            result.Message = result.IsConnected ? "Connection successful" : "Cannot connect";
        }
        catch (Exception ex)
        {
            result.Message = $"Error: {ex.Message}";
        }

        return result;
    }

    private async Task<string> GetDatabaseNameAsync()
    {
        try
        {
            var connection = _dbContext.Database.GetDbConnection();
            return await Task.FromResult(connection.Database);
        }
        catch
        {
            return "Unknown";
        }
    }

    private async Task<int> GetTableCountAsync()
    {
        try
        {
            // Contar las DbSets públicas del contexto
            var dbSetProperties = _dbContext.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsGenericType &&
                           p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Count();

            return await Task.FromResult(dbSetProperties);
        }
        catch
        {
            return 0;
        }
    }

    private static string MaskConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "Not configured";

        // Ocultar password y información sensible
        var masked = connectionString;
        
        // Password
        masked = System.Text.RegularExpressions.Regex.Replace(
            masked, 
            @"Password=([^;]+)", 
            "Password=***", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // User Id
        masked = System.Text.RegularExpressions.Regex.Replace(
            masked,
            @"User Id=([^;]+)",
            "User Id=***",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // Username (PostgreSQL)
        masked = System.Text.RegularExpressions.Regex.Replace(
            masked,
            @"Username=([^;]+)",
            "Username=***",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return masked;
    }
}

#region Response Models

/// <summary>
/// Respuesta del health check de base de datos con información detallada de diagnóstico.
/// </summary>
public class DatabaseHealthResponse
{
    /// <summary>
    /// Proveedor de base de datos utilizado (Oracle, SqlServer, PostgreSQL).
    /// </summary>
    /// <example>SqlServer</example>
    public string Provider { get; set; } = string.Empty;
    
    /// <summary>
    /// Indica si la conexión a la base de datos fue exitosa.
    /// </summary>
    /// <example>true</example>
    public bool IsConnected { get; set; }
    
    /// <summary>
    /// Estado general de la base de datos (Healthy, Unhealthy).
    /// </summary>
    /// <example>Healthy</example>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Mensaje descriptivo del resultado de la verificación.
    /// </summary>
    /// <example>Successfully connected to SqlServer database</example>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Nombre de la base de datos conectada (si la conexión fue exitosa).
    /// </summary>
    /// <example>ElectroHuila_Dev</example>
    public string? DatabaseName { get; set; }
    
    /// <summary>
    /// Cantidad de tablas detectadas en el DbContext (si la conexión fue exitosa).
    /// </summary>
    /// <example>22</example>
    public int? TableCount { get; set; }
    
    /// <summary>
    /// Preview del connection string con información sensible enmascarada.
    /// </summary>
    /// <example>Server=localhost;Database=***;User Id=***;Password=***</example>
    public string ConnectionStringPreview { get; set; } = string.Empty;
    
    /// <summary>
    /// Detalles del error si la conexión falló (tipo de excepción).
    /// </summary>
    /// <example>SqlException</example>
    public string? ErrorDetails { get; set; }
    
    /// <summary>
    /// Timestamp UTC de cuándo se realizó el health check.
    /// </summary>
    /// <example>2025-10-20T15:30:00Z</example>
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Respuesta con información del proveedor de base de datos configurado.
/// </summary>
public class DatabaseInfoResponse
{
    /// <summary>
    /// Proveedor de base de datos actualmente configurado.
    /// </summary>
    /// <example>Oracle</example>
    public string CurrentProvider { get; set; } = string.Empty;
    
    /// <summary>
    /// Preview del connection string con credenciales enmascaradas.
    /// </summary>
    /// <example>User Id=***;Password=***;Data Source=(DESCRIPTION=...)</example>
    public string ConnectionStringPreview { get; set; } = string.Empty;
    
    /// <summary>
    /// Lista de proveedores de base de datos disponibles en el sistema.
    /// </summary>
    /// <example>["Oracle", "SqlServer", "PostgreSQL"]</example>
    public string[] AvailableProviders { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// Mensaje informativo sobre el proveedor actual.
    /// </summary>
    /// <example>Currently configured to use Oracle database</example>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Respuesta con los resultados de pruebas de conectividad a todos los proveedores.
/// </summary>
public class AllProvidersTestResponse
{
    /// <summary>
    /// Lista de resultados de prueba para cada proveedor.
    /// </summary>
    public List<ProviderTestResult> Results { get; set; } = new();
    
    /// <summary>
    /// Timestamp UTC de cuándo se ejecutaron las pruebas.
    /// </summary>
    /// <example>2025-10-20T15:45:00Z</example>
    public DateTime TestedAt { get; set; }
}

/// <summary>
/// Resultado de prueba de conectividad para un proveedor específico.
/// </summary>
public class ProviderTestResult
{
    /// <summary>
    /// Nombre del proveedor probado (Oracle, SqlServer, PostgreSQL).
    /// </summary>
    /// <example>Oracle</example>
    public string Provider { get; set; } = string.Empty;
    
    /// <summary>
    /// Indica si el connection string está configurado en appsettings.json.
    /// </summary>
    /// <example>true</example>
    public bool IsConfigured { get; set; }
    
    /// <summary>
    /// Indica si la conexión al proveedor fue exitosa.
    /// </summary>
    /// <example>true</example>
    public bool IsConnected { get; set; }
    
    /// <summary>
    /// Mensaje descriptivo del resultado de la prueba.
    /// </summary>
    /// <example>Connection successful</example>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Preview del connection string con información sensible enmascarada (si está configurado).
    /// </summary>
    /// <example>User Id=***;Password=***;Data Source=...</example>
    public string? ConnectionStringPreview { get; set; }
}

#endregion
