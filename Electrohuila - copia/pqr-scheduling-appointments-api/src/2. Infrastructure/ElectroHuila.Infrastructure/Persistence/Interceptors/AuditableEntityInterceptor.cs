using ElectroHuila.Application.Common.Interfaces.Services.Common;
using ElectroHuila.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ElectroHuila.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor de Entity Framework que maneja automáticamente los campos de auditoría en entidades.
/// Hereda de <see cref="SaveChangesInterceptor"/> para interceptar operaciones de guardado.
/// </summary>
/// <remarks>
/// Este interceptor se ejecuta automáticamente antes de cada operación SaveChanges/SaveChangesAsync
/// en el DbContext y actualiza los campos de auditoría (CreatedAt, UpdatedAt) en todas las entidades
/// que heredan de <see cref="BaseEntity"/>. También implementa soft delete convirtiendo eliminaciones
/// físicas en eliminaciones lógicas estableciendo IsActive = false.
/// </remarks>
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly IDateTimeProvider? _dateTimeProvider;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AuditableEntityInterceptor"/>.
    /// </summary>
    /// <param name="currentUserService">Servicio para obtener información del usuario actual (opcional).</param>
    /// <param name="dateTimeProvider">Proveedor de fecha y hora para auditoría (opcional).</param>
    /// <remarks>
    /// Los servicios son opcionales para permitir flexibilidad en diferentes contextos.
    /// Si no se proporcionan, se utilizarán valores por defecto (DateTime.UtcNow).
    /// </remarks>
    public AuditableEntityInterceptor(
        ICurrentUserService? currentUserService,
        IDateTimeProvider? dateTimeProvider)
    {
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <summary>
    /// Intercepta la operación SaveChanges síncrona para actualizar campos de auditoría.
    /// </summary>
    /// <param name="eventData">Datos del evento que contienen el contexto de la base de datos.</param>
    /// <param name="result">Resultado de la interceptación.</param>
    /// <returns>Resultado de la interceptación después de procesar las entidades.</returns>
    /// <remarks>
    /// Se ejecuta antes de que Entity Framework persista los cambios en la base de datos.
    /// </remarks>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Intercepta la operación SaveChangesAsync asíncrona para actualizar campos de auditoría.
    /// </summary>
    /// <param name="eventData">Datos del evento que contienen el contexto de la base de datos.</param>
    /// <param name="result">Resultado de la interceptación.</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Task que representa el resultado de la interceptación después de procesar las entidades.</returns>
    /// <remarks>
    /// Versión asíncrona del interceptor que mantiene la misma funcionalidad.
    /// </remarks>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Actualiza automáticamente los campos de auditoría en todas las entidades auditables que han cambiado.
    /// </summary>
    /// <param name="context">Contexto de Entity Framework que contiene las entidades a procesar.</param>
    /// <remarks>
    /// Este método procesa las entidades según su estado:
    /// 
    /// <para><strong>EntityState.Added:</strong></para>
    /// - Establece CreatedAt y UpdatedAt con la fecha/hora actual
    /// 
    /// <para><strong>EntityState.Modified:</strong></para>
    /// - Actualiza únicamente UpdatedAt con la fecha/hora actual
    /// 
    /// <para><strong>EntityState.Deleted:</strong></para>
    /// - Implementa soft delete: cambia el estado a Modified
    /// - Establece IsActive = false en lugar de eliminar físicamente
    /// - Actualiza UpdatedAt con la fecha/hora actual
    /// 
    /// Solo procesa entidades que heredan de <see cref="BaseEntity"/> y que estén activas para eliminación.
    /// </remarks>
    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var now = _dateTimeProvider?.UtcNow ?? System.DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;

                case EntityState.Deleted:
                    if (entry.Entity.IsActive)
                    {
                        entry.State = EntityState.Modified;
                        entry.Entity.IsActive = false;
                        entry.Entity.UpdatedAt = now;
                    }
                    break;
            }
        }
    }
}