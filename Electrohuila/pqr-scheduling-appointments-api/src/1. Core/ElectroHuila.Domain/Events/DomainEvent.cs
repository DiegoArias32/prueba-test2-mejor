namespace ElectroHuila.Domain.Events;

/// <summary>
/// Clase base abstracta para todos los eventos del dominio
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <summary>
    /// Identificador único del evento generado automáticamente
    /// </summary>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Fecha y hora en que ocurrió el evento en formato UTC
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <summary>
    /// Tipo de evento (debe ser implementado por las clases derivadas)
    /// </summary>
    public abstract string EventType { get; }
}