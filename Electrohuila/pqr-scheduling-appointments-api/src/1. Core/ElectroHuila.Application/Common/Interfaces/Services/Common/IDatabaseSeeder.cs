namespace ElectroHuila.Application.Common.Interfaces.Services.Common;

/// <summary>
/// Interfaz para el servicio de inicialización de datos base del sistema.
/// Proporciona métodos para poblar la base de datos con datos semilla necesarios.
/// </summary>
public interface IDatabaseSeeder
{
    /// <summary>
    /// Ejecuta el proceso completo de inicialización de la base de datos.
    /// Crea roles, permisos, formularios, módulos y el usuario administrador inicial.
    /// </summary>
    /// <returns>Una tarea que representa la operación asíncrona de seeding.</returns>
    Task SeedAsync();
}
