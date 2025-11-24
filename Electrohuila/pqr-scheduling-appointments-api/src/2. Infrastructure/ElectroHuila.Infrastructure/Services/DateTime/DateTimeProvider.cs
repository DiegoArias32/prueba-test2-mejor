using ElectroHuila.Application.Common.Interfaces;
using ElectroHuila.Application.Common.Interfaces.Services.Common;

namespace ElectroHuila.Infrastructure.Services.DateTime;

/// <summary>
/// Servicio que proporciona fecha y hora del sistema.
/// Permite hacer testing más fácil al poder reemplazar el tiempo real con valores de prueba.
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    /// <summary>
    /// Fecha y hora actual en UTC (recomendado para guardar en base de datos).
    /// </summary>
    public System.DateTime UtcNow => System.DateTime.UtcNow;

    /// <summary>
    /// Fecha y hora actual en zona horaria local del servidor.
    /// </summary>
    public System.DateTime Now => System.DateTime.Now;

    /// <summary>
    /// Solo la fecha de hoy (sin hora) en zona horaria local.
    /// </summary>
    public DateOnly Today => DateOnly.FromDateTime(System.DateTime.Today);

    /// <summary>
    /// Solo la fecha de hoy (sin hora) en UTC.
    /// </summary>
    public DateOnly UtcToday => DateOnly.FromDateTime(System.DateTime.UtcNow);

    /// <summary>
    /// Solo la hora actual (sin fecha) en zona horaria local.
    /// </summary>
    public TimeOnly CurrentTime => TimeOnly.FromDateTime(System.DateTime.Now);

    /// <summary>
    /// Solo la hora actual (sin fecha) en UTC.
    /// </summary>
    public TimeOnly UtcCurrentTime => TimeOnly.FromDateTime(System.DateTime.UtcNow);
}