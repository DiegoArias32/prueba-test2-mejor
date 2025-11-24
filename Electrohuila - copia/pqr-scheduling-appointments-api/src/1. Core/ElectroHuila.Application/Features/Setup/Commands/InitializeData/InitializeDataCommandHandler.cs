using ElectroHuila.Application.Common.Interfaces.Services.Common;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Setup.Commands.InitializeData;

/// <summary>
/// Manejador del comando para inicializar datos base del sistema.
/// Ejecuta el DatabaseSeeder para crear roles, permisos, formularios, módulos y el usuario admin.
/// </summary>
public class InitializeDataCommandHandler : IRequestHandler<InitializeDataCommand, Result<object>>
{
    private readonly IDatabaseSeeder _databaseSeeder;

    public InitializeDataCommandHandler(IDatabaseSeeder databaseSeeder)
    {
        _databaseSeeder = databaseSeeder;
    }

    public async Task<Result<object>> Handle(InitializeDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Ejecutar el seeder que crea todos los datos base
            await _databaseSeeder.SeedAsync();

            var result = new
            {
                message = "Database initialization completed successfully",
                tablesSeeded = new[]
                {
                    "Permissions (3 registros)",
                    "Modules (5 registros)",
                    "Forms (12 registros)",
                    "Roles (4 registros)",
                    "FormModules (relaciones)",
                    "RolFormPermis (permisos admin)",
                    "Users (usuario admin)",
                    "RolUsers (admin role)"
                },
                adminCredentials = new
                {
                    username = "admin",
                    password = "Admin123!",
                    note = "Change password after first login"
                }
            };

            return Result.Success<object>(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<object>($"Error initializing database: {ex.Message}");
        }
    }
}
