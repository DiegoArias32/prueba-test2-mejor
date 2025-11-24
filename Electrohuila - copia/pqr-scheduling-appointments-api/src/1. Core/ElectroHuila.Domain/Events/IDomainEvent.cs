namespace ElectroHuila.Domain.Events;

/// <summary>
/// Interfaz base para todos los eventos del dominio
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Identificador único del evento
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Fecha y hora en que ocurrió el evento
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Tipo de evento
    /// </summary>
    string EventType { get; }
}