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
                throw new InvalidOperationException($"Connection string for provider {provider} is null or empty");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            switch (provider)
            {
                case Services.DatabaseProvider.Oracle:
                    optionsBuilder.UseOracle(connectionString)
                        .EnableSensitiveDataLogging()
                        .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
                    break;

                case Services.DatabaseProvider.SqlServer:
                    optionsBuilder.UseSqlServer(connectionString);
                    break;

                case Services.DatabaseProvider.PostgreSQL:
                    optionsBuilder.UseNpgsql(connectionString);
                    break;

                case Services.DatabaseProvider.MySQL:
                    var serverVersion = ServerVersion.AutoDetect(connectionString);
                    optionsBuilder.UseMySql(connectionString, serverVersion);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported database provider: {provider}");
            }

            var context = new ApplicationDbContext(optionsBuilder.Options);

            // Verificar que la conexión esté configurada
            if (context.Database.GetDbConnection() == null)
            {
                throw new InvalidOperationException("Database connection was not initialized properly");
            }

            return context;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
