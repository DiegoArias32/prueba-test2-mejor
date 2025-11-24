namespace ElectroHuila.Domain.Enums;

/// <summary>
/// Enumeración de tipos de instalación eléctrica
/// </summary>
public enum InstallationType
{
    /// <summary>
    /// Nueva conexión eléctrica
    /// </summary>
    NewConnection = 1,

    /// <summary>
    /// Reconexión de servicio
    /// </summary>
    Reconnection = 2,

    /// <summary>
    /// Reemplazo de medidor
    /// </summary>
    MeterReplacement = 3,

    /// <summary>
    /// Actualización de servicio
    /// </summary>
    ServiceUpgrade = 4,

    /// <summary>
    /// Conexión temporal
    /// </summary>
    TemporaryConnection = 5,

    /// <summary>
    /// Aumento de potencia
    /// </summary>
    PowerIncrease = 6,

    /// <summary>
    /// Disminución de potencia
    /// </summary>
    PowerDecrease = 7
}

/// <summary>
/// Métodos de extensión para el enum InstallationType
/// </summary>
public static class InstallationTypeExtensions
{
    /// <summary>
    /// Convierte el tipo de instalación a su representación en cadena para mostrar
    /// </summary>
    /// <param name="installationType">Tipo de instalación</param>
    /// <returns>Representación en cadena del tipo de instalación</returns>
    public static string ToDisplayString(this InstallationType installationType)
    {
        return installationType switch
        {
            InstallationType.NewConnection => "Nueva Conexión",
            InstallationType.Reconnection => "Reconexión",
            InstallationType.MeterReplacement => "Cambio de Medidor",
            InstallationType.ServiceUpgrade => "Actualización de Servicio",
            InstallationType.TemporaryConnection => "Conexión Temporal",
            InstallationType.PowerIncrease => "Aumento de Potencia",
            InstallationType.PowerDecrease => "Disminución de Potencia",
            _ => installationType.ToString()
        };
    }

    /// <summary>
    /// Obtiene una descripción detallada del tipo de instalación
    /// </summary>
    /// <param name="installationType">Tipo de instalación</param>
    /// <returns>Descripción detallada del tipo de instalación</returns>
    public static string GetDescription(this InstallationType installationType)
    {
        return installationType switch
        {
            InstallationType.NewConnection => "Instalación de nueva acometida eléctrica para inmueble sin servicio",
            InstallationType.Reconnection => "Reconexión del servicio eléctrico previamente suspendido",
            InstallationType.MeterReplacement => "Reemplazo del medidor de energía eléctrica",
            InstallationType.ServiceUpgrade => "Actualización del servicio eléctrico existente",
            InstallationType.TemporaryConnection => "Conexión temporal para eventos o construcciones",
            InstallationType.PowerIncrease => "Incremento en la capacidad de potencia contratada",
            InstallationType.PowerDecrease => "Reducción en la capacidad de potencia contratada",
            _ => installationType.ToString()
        };
    }

    /// <summary>
    /// Obtiene la duración estimada en horas para este tipo de instalación
    /// </summary>
    /// <param name="installationType">Tipo de instalación</param>
    /// <returns>Duración estimada en horas</returns>
    public static int GetEstimatedDurationHours(this InstallationType installationType)
    {
        return installationType switch
        {
            InstallationType.NewConnection => 4,
            InstallationType.Reconnection => 1,
            InstallationType.MeterReplacement => 2,
            InstallationType.ServiceUpgrade => 3,
            InstallationType.TemporaryConnection => 2,
            InstallationType.PowerIncrease => 3,
            InstallationType.PowerDecrease => 2,
            _ => 2
        };
    }

    /// <summary>
    /// Determina si este tipo de instalación requiere documentación adicional
    /// </summary>
    /// <param name="installationType">Tipo de instalación</param>
    /// <returns>True si requiere documentación, false en caso contrario</returns>
    public static bool RequiresDocumentation(this InstallationType installationType)
    {
        return installationType switch
        {
            InstallationType.NewConnection => true,
            InstallationType.Reconnection => false,
            InstallationType.MeterReplacement => false,
            InstallationType.ServiceUpgrade => true,
            InstallationType.TemporaryConnection => true,
            InstallationType.PowerIncrease => true,
            InstallationType.PowerDecrease => true,
            _ => false
        };
    }
}