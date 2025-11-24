namespace ElectroHuila.Application.Common.Interfaces.Services.Common;

/// <summary>
/// Servicio para obtener la fecha y hora actual de manera testeable
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Obtiene la fecha y hora actual en UTC
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Obtiene la fecha y hora actual local
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    /// Obtiene la fecha actual local (solo fecha)
    /// </summary>
    DateOnly Today { get; }

    /// <summary>
    /// Obtiene la fecha actual en UTC (solo fecha)
    /// </summary>
    DateOnly UtcToday { get; }

    /// <summary>
    /// Obtiene la hora actual local (solo hora)
    /// </summary>
    TimeOnly CurrentTime { get; }

    /// <summary>
    /// Obtiene la hora actual en UTC (solo hora)
    /// </summary>
    TimeOnly UtcCurrentTime { get; }
}