using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace ElectroHuila.Infrastructure.Persistence;

/// <summary>
/// Factory para crear instancias de ApplicationDbContext con el proveedor de BD correcto.
/// </summary>
public interface IDbContextFactory
{
    /// <summary>
    /// Crea una nueva instancia de ApplicationDbContext configurada con el proveedor actual.
    /// </summary>
    ApplicationDbContext CreateDbContext();
}

/// <summary>
/// Implementación del factory de DbContext con selección dinámica de proveedor.
/// </summary>
public class ApplicationDbContextFactory : IDbContextFactory
{
    private readonly Services.IDatabaseProviderService _databaseProviderService;

    public ApplicationDbContextFactory(Services.IDatabaseProviderService databaseProviderService)
    {
        _databaseProviderService = databaseProviderService;
    }

    public ApplicationDbContext CreateDbContext()
    {
        try
        {
            var provider = _databaseProviderService.CurrentProvider;
            var connectionString = _databaseProviderService.GetConnectionString();

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine($"[DbContextFactory] ERROR: Connection string for provider {provider} is null or empty");
                throw new InvalidOperationException($"Connection string for provider {provider} is null or empty");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            Console.WriteLine($"[DbContextFactory] Creating DbContext with provider: {provider}");
            Console.WriteLine($"[DbContextFactory] Connection string length: {connectionString.Length}");
            Console.WriteLine($"[DbContextFactory] Connection string preview: {connectionString.Substring(0, Math.Min(50, connectionString.Length))}...");

            switch (provider)
            {
                case Services.DatabaseProvider.Oracle:
                    optionsBuilder.UseOracle(connectionString);
                    Console.WriteLine($"[DbContextFactory] Oracle configured successfully");
                    break;

                case Services.DatabaseProvider.SqlServer:
                    optionsBuilder.UseSqlServer(connectionString);
                    Console.WriteLine($"[DbContextFactory] SQL Server configured successfully");
                    break;

                case Services.DatabaseProvider.PostgreSQL:
                    optionsBuilder.UseNpgsql(connectionString);
                    Console.WriteLine($"[DbContextFactory] PostgreSQL configured successfully");
                    break;

                case Services.DatabaseProvider.MySQL:
                    var serverVersion = ServerVersion.AutoDetect(connectionString);
                    optionsBuilder.UseMySql(connectionString, serverVersion);
                    Console.WriteLine($"[DbContextFactory] MySQL configured successfully");
                    break;

                default:
                    Console.WriteLine($"[DbContextFactory] ERROR: Unsupported database provider: {provider}");
                    throw new InvalidOperationException($"Unsupported database provider: {provider}");
            }

            var context = new ApplicationDbContext(optionsBuilder.Options);

            // Verificar que la conexión esté configurada
            if (context.Database.GetDbConnection() == null)
            {
                Console.WriteLine($"[DbContextFactory] ERROR: Database connection was not initialized properly");
                throw new InvalidOperationException("Database connection was not initialized properly");
            }

            Console.WriteLine($"[DbContextFactory] DbContext created successfully with valid database connection");
            return context;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DbContextFactory] CRITICAL ERROR creating DbContext:");
            Console.WriteLine($"[DbContextFactory]   Exception Type: {ex.GetType().Name}");
            Console.WriteLine($"[DbContextFactory]   Message: {ex.Message}");
            Console.WriteLine($"[DbContextFactory]   Stack Trace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"[DbContextFactory]   Inner Exception: {ex.InnerException.GetType().Name}");
                Console.WriteLine($"[DbContextFactory]   Inner Message: {ex.InnerException.Message}");
            }

            throw;
        }
    }
}
