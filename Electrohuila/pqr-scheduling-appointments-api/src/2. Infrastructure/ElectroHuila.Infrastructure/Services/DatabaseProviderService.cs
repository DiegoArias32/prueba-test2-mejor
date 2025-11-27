using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ElectroHuila.Infrastructure.Services;

/// <summary>
/// Enumeración de proveedores de base de datos soportados.
/// </summary>
public enum DatabaseProvider
{
    /// <summary>
    /// Oracle Database (Oracle RDS o Oracle Cloud Autonomous Database).
    /// </summary>
    Oracle,
    
    /// <summary>
    /// Microsoft SQL Server.
    /// </summary>
    SqlServer,
    
    /// <summary>
    /// PostgreSQL Database.
    /// </summary>
    PostgreSQL,
    
    /// <summary>
    /// MySQL Database.
    /// </summary>
    MySQL
}

/// <summary>
/// Servicio para gestionar el proveedor de base de datos dinámicamente.
/// </summary>
public interface IDatabaseProviderService
{
    /// <summary>
    /// Obtiene el proveedor de base de datos actual.
    /// </summary>
    DatabaseProvider CurrentProvider { get; }
    
    /// <summary>
    /// Obtiene la cadena de conexión para el proveedor actual.
    /// </summary>
    string GetConnectionString();
    
    /// <summary>
    /// Establece el proveedor de base de datos para el contexto actual.
    /// </summary>
    /// <param name="provider">El proveedor a utilizar.</param>
    void SetProvider(DatabaseProvider provider);
}

/// <summary>
/// Implementación del servicio de proveedor de base de datos.
/// Scoped para permitir cambios por request HTTP.
/// </summary>
public class DatabaseProviderService : IDatabaseProviderService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private DatabaseProvider? _overrideProvider;

    public DatabaseProviderService(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public DatabaseProvider CurrentProvider
    {
        get
        {
            // 1. Si hay un override en este request, usarlo
            if (_overrideProvider.HasValue)
            {
                return _overrideProvider.Value;
            }

            // 2. Verificar si hay un proveedor en HttpContext.Items
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Items.ContainsKey("DatabaseProvider") == true)
            {
                var providerString = httpContext.Items["DatabaseProvider"]?.ToString();
                if (!string.IsNullOrEmpty(providerString))
                {
                    var parsed = ParseProvider(providerString);
                    if (parsed.HasValue)
                    {
                        return parsed.Value;
                    }
                }
            }

            // 3. Usar el proveedor por defecto de la configuración
            var defaultProviderString = _configuration["DatabaseProvider"] ?? "Oracle";
            return ParseProvider(defaultProviderString) ?? DatabaseProvider.Oracle;
        }
    }

    public string GetConnectionString()
    {
        var provider = CurrentProvider;
        
        return provider switch
        {
            DatabaseProvider.Oracle => _configuration.GetConnectionString("OracleConnection")
                ?? throw new InvalidOperationException("OracleConnection string not configured"),
            
            DatabaseProvider.SqlServer => _configuration.GetConnectionString("SqlServerConnection")
                ?? throw new InvalidOperationException("SqlServerConnection string not configured"),
            
            DatabaseProvider.PostgreSQL => _configuration.GetConnectionString("PostgreSqlConnection")
                ?? throw new InvalidOperationException("PostgreSqlConnection string not configured"),
            
            DatabaseProvider.MySQL => _configuration.GetConnectionString("MySqlConnection")
                ?? throw new InvalidOperationException("MySqlConnection string not configured"),
            
            _ => throw new InvalidOperationException($"Unsupported database provider: {provider}")
        };
    }

    public void SetProvider(DatabaseProvider provider)
    {
        _overrideProvider = provider;

        // También almacenar en HttpContext.Items si está disponible
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            httpContext.Items["DatabaseProvider"] = provider.ToString();
        }
    }

    private static DatabaseProvider? ParseProvider(string providerString)
    {
        return providerString.ToUpperInvariant() switch
        {
            "ORACLE" => DatabaseProvider.Oracle,
            "SQLSERVER" => DatabaseProvider.SqlServer,
            "POSTGRESQL" => DatabaseProvider.PostgreSQL,
            "MYSQL" => DatabaseProvider.MySQL,
            _ => null
        };
    }
}

