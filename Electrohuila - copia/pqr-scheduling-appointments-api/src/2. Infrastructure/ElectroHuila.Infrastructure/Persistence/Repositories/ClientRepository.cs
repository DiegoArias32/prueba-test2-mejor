using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Clients;
using ElectroHuila.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio para la gestión de clientes en la base de datos.
/// Implementa <see cref="IClientRepository"/> para proporcionar operaciones específicas de la entidad <see cref="Client"/>.
/// </summary>
/// <remarks>
/// Este repositorio maneja todas las operaciones de persistencia para clientes, incluyendo búsquedas
/// por diferentes identificadores únicos (ID, número de cliente, documento, email), validaciones de existencia,
/// generación automática de números de cliente y gestión de soft delete. Todas las consultas incluyen
/// automáticamente las citas asociadas a cada cliente.
/// </remarks>
public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ClientRepository"/>.
    /// </summary>
    /// <param name="context">Contexto de la base de datos de la aplicación.</param>
    public ClientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene un cliente por su identificador único incluyendo sus citas asociadas.
    /// </summary>
    /// <param name="id">Identificador único del cliente.</param>
    /// <returns>
    /// El cliente encontrado con sus citas si existe y está activo; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// Incluye automáticamente todas las citas asociadas al cliente mediante Include.
    /// Solo considera clientes activos (IsActive = true).
    /// </remarks>
    public async Task<Client?> GetByIdAsync(int id)
    {
        return await _context.Clients
            .Include(c => c.Appointments)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
    }

    /// <summary>
    /// Busca un cliente por su número de cliente único incluyendo sus citas asociadas.
    /// </summary>
    /// <param name="clientNumber">Número único del cliente generado automáticamente por el sistema.</param>
    /// <returns>
    /// El cliente encontrado con sus citas si existe y está activo; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// El número de cliente sigue el formato "CLI-YYYYMMDD-{GUID}" y es único en el sistema.
    /// Incluye automáticamente todas las citas asociadas al cliente.
    /// </remarks>
    public async Task<Client?> GetByClientNumberAsync(string clientNumber)
    {
        return await _context.Clients
            .Include(c => c.Appointments)
            .FirstOrDefaultAsync(c => c.ClientNumber == clientNumber && c.IsActive);
    }

    /// <summary>
    /// Busca un cliente por su número de documento de identidad incluyendo sus citas asociadas.
    /// </summary>
    /// <param name="documentNumber">Número de documento de identidad del cliente.</param>
    /// <returns>
    /// El cliente encontrado con sus citas si existe y está activo; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// El número de documento debe ser único para cada cliente activo en el sistema.
    /// Incluye automáticamente todas las citas asociadas al cliente.
    /// </remarks>
    public async Task<Client?> GetByDocumentNumberAsync(string documentNumber)
    {
        return await _context.Clients
            .Include(c => c.Appointments)
            .FirstOrDefaultAsync(c => c.DocumentNumber == documentNumber && c.IsActive);
    }

    /// <summary>
    /// Obtiene todos los clientes activos del sistema.
    /// </summary>
    /// <returns>
    /// Colección de todos los clientes que tienen IsActive = true.
    /// </returns>
    /// <remarks>
    /// No incluye las citas asociadas para optimizar el rendimiento en consultas masivas.
    /// Solo retorna clientes activos, excluyendo los eliminados lógicamente.
    /// </remarks>
    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        return await _context.Clients
            .Where(c => c.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los clientes del sistema incluyendo inactivos.
    /// </summary>
    /// <returns>
    /// Colección de todos los clientes (activos e inactivos).
    /// </returns>
    /// <remarks>
    /// No incluye las citas asociadas para optimizar el rendimiento en consultas masivas.
    /// NO filtra por IsActive, retorna todos los registros.
    /// </remarks>
    public async Task<IEnumerable<Client>> GetAllIncludingInactiveAsync()
    {
        return await _context.Clients.ToListAsync();
    }

    /// <summary>
    /// Agrega un nuevo cliente al sistema con generación automática de número de cliente.
    /// </summary>
    /// <param name="client">Entidad cliente a agregar al sistema.</param>
    /// <returns>
    /// El cliente agregado con su número de cliente generado y ID asignado.
    /// </returns>
    /// <remarks>
    /// Genera automáticamente un número de cliente único antes de persistir.
    /// El número sigue el formato "CLI-YYYYMMDD-{GUID}" para garantizar unicidad.
    /// Guarda los cambios automáticamente en la base de datos.
    /// </remarks>
    public async Task<Client> AddAsync(Client client)
    {
        client.ClientNumber = GenerateClientNumber();
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client;
    }

    /// <summary>
    /// Actualiza la información de un cliente existente en el sistema.
    /// </summary>
    /// <param name="client">Entidad cliente con los datos actualizados.</param>
    /// <returns>Task que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Actualiza todos los campos modificados del cliente.
    /// Guarda los cambios automáticamente en la base de datos.
    /// El cliente debe existir previamente en el contexto o ser rastreado por Entity Framework.
    /// </remarks>
    public async Task UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Elimina lógicamente un cliente estableciendo IsActive = false.
    /// </summary>
    /// <param name="id">Identificador único del cliente a eliminar.</param>
    /// <returns>Task que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Implementa soft delete para preservar el historial del cliente y sus relaciones.
    /// Las citas asociadas al cliente se mantienen para auditoría y referencias históricas.
    /// Si el cliente no existe, no se realiza ninguna acción.
    /// Guarda los cambios automáticamente en la base de datos.
    /// </remarks>
    public async Task DeleteAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client != null)
        {
            client.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Verifica si existe un cliente activo con el identificador especificado.
    /// </summary>
    /// <param name="id">Identificador único del cliente a verificar.</param>
    /// <returns>
    /// true si existe un cliente activo con ese ID; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// Solo considera clientes activos para la verificación de existencia.
    /// Útil para validaciones antes de operaciones que requieren un cliente existente.
    /// </remarks>
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Clients.AnyAsync(c => c.Id == id && c.IsActive);
    }

    /// <summary>
    /// Verifica si existe un cliente activo con el número de documento especificado.
    /// </summary>
    /// <param name="documentNumber">Número de documento a verificar.</param>
    /// <returns>
    /// true si existe un cliente activo con ese documento; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// Esencial para validar unicidad de documentos antes de crear nuevos clientes.
    /// Solo considera clientes activos para permitir reutilización de documentos de clientes eliminados.
    /// </remarks>
    public async Task<bool> ExistsByDocumentNumberAsync(string documentNumber)
    {
        return await _context.Clients.AnyAsync(c => c.DocumentNumber == documentNumber && c.IsActive);
    }

    /// <summary>
    /// Busca un cliente por su número de documento incluyendo sus citas asociadas.
    /// </summary>
    /// <param name="documentNumber">Número de documento del cliente a buscar.</param>
    /// <returns>
    /// El cliente encontrado con sus citas si existe y está activo; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// Método alternativo a GetByDocumentNumberAsync con la misma funcionalidad.
    /// Incluye automáticamente todas las citas asociadas al cliente.
    /// </remarks>
    public async Task<Client?> GetByDocumentAsync(string documentNumber)
    {
        return await _context.Clients
            .Include(c => c.Appointments)
            .FirstOrDefaultAsync(c => c.DocumentNumber == documentNumber && c.IsActive);
    }

    /// <summary>
    /// Busca un cliente por su dirección de correo electrónico incluyendo sus citas asociadas.
    /// </summary>
    /// <param name="email">Dirección de correo electrónico del cliente.</param>
    /// <returns>
    /// El cliente encontrado con sus citas si existe y está activo; de lo contrario, null.
    /// </returns>
    /// <remarks>
    /// Útil para autenticación o búsquedas por email.
    /// Incluye automáticamente todas las citas asociadas al cliente.
    /// </remarks>
    public async Task<Client?> GetByEmailAsync(string email)
    {
        return await _context.Clients
            .Include(c => c.Appointments)
            .FirstOrDefaultAsync(c => c.Email == email && c.IsActive);
    }

    /// <summary>
    /// Verifica si existe un cliente activo con la dirección de email especificada.
    /// </summary>
    /// <param name="email">Dirección de correo electrónico a verificar.</param>
    /// <returns>
    /// true si existe un cliente activo con ese email; de lo contrario, false.
    /// </returns>
    /// <remarks>
    /// Importante para validar unicidad de emails si el sistema lo requiere.
    /// Solo considera clientes activos para la verificación.
    /// </remarks>
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Clients.AnyAsync(c => c.Email == email && c.IsActive);
    }

    /// <summary>
    /// Genera un número único de cliente con formato estándar del sistema.
    /// </summary>
    /// <returns>
    /// Número de cliente único con formato "CLI-YYYYMMDD-{GUID}".
    /// </returns>
    /// <remarks>
    /// El formato incluye:
    /// - Prefijo "CLI" para identificar el tipo de entidad
    /// - Fecha actual en formato YYYYMMDD para ordenamiento cronológico
    /// - GUID sin guiones para garantizar unicidad absoluta
    /// 
    /// Ejemplo: "CLI-20241002-a1b2c3d4e5f6789012345678901234ab"
    /// </remarks>
    private static string GenerateClientNumber()
    {
        return $"CLI-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}";
    }
}