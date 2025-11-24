using ElectroHuila.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ElectroHuila.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor de Entity Framework que implementa eliminación lógica (soft delete) para entidades.
/// Hereda de <see cref="SaveChangesInterceptor"/> para interceptar operaciones de eliminación.
/// </summary>
/// <remarks>
/// Este interceptor convierte automáticamente todas las operaciones de eliminación física (DELETE)
/// en eliminaciones lógicas estableciendo el campo IsActive = false en lugar de eliminar
/// físicamente los registros de la base de datos. Esto permite mantener un historial completo
/// de datos y facilita la auditoría y recuperación de información.
/// 
/// Solo procesa entidades que heredan de <see cref="BaseEntity"/>.
/// </remarks>
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepta la operación SaveChanges síncrona para implementar soft delete.
    /// </summary>
    /// <param name="eventData">Datos del evento que contienen el contexto de la base de datos.</param>
    /// <param name="result">Resultado de la interceptación.</param>
    /// <returns>Resultado de la interceptación después de procesar las eliminaciones.</returns>
    /// <remarks>
    /// Se ejecuta antes de que Entity Framework persista los cambios en la base de datos,
    /// convirtiendo las eliminaciones físicas en eliminaciones lógicas.
    /// </remarks>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            UpdateSoftDeleteStatuses(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Intercepta la operación SaveChangesAsync asíncrona para implementar soft delete.
    /// </summary>
    /// <param name="eventData">Datos del evento que contienen el contexto de la base de datos.</param>
    /// <param name="result">Resultado de la interceptación.</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Task que representa el resultado de la interceptación después de procesar las eliminaciones.</returns>
    /// <remarks>
    /// Versión asíncrona del interceptor que mantiene la misma funcionalidad de soft delete.
    /// </remarks>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateSoftDeleteStatuses(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Convierte todas las eliminaciones físicas en eliminaciones lógicas para entidades auditables.
    /// </summary>
    /// <param name="context">Contexto de Entity Framework que contiene las entidades a procesar.</param>
    /// <remarks>
    /// Este método:
    /// 1. Identifica todas las entidades marcadas para eliminación (EntityState.Deleted)
    /// 2. Cambia su estado a Modified en lugar de Deleted
    /// 3. Establece IsActive = false para marcarlas como eliminadas lógicamente
    /// 
    /// <para><strong>Ventajas del soft delete:</strong></para>
    /// - Preserva el historial de datos para auditoría
    /// - Permite recuperación de registros eliminados accidentalmente
    /// - Mantiene la integridad referencial en relaciones complejas
    /// - Facilita el cumplimiento de regulaciones de retención de datos
    /// 
    /// <para><strong>Nota:</strong></para>
    /// BaseEntity no incluye un campo DeletedAt, solo se marca como inactiva.
    /// </remarks>
    private static void UpdateSoftDeleteStatuses(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsActive = false;
                // BaseEntity doesn't have DeletedAt property, just mark as inactive
            }
        }
    }
}