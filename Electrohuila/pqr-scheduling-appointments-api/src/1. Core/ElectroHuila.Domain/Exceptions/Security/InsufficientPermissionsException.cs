namespace ElectroHuila.Domain.Exceptions.Security;

/// <summary>
/// Excepción que se lanza cuando el usuario no tiene permisos suficientes
/// </summary>
public sealed class InsufficientPermissionsException : DomainException
{
    /// <summary>
    /// Constructor para excepción de permisos insuficientes
    /// </summary>
    /// <param name="resource">Recurso al que se intenta acceder</param>
    /// <param name="action">Acción que se intenta realizar</param>
    public InsufficientPermissionsException(string resource, string action)
        : base("INSUFFICIENT_PERMISSIONS",
               $"Insufficient permissions to {action} {resource}.",
               new { Resource = resource, Action = action })
    {
    }

    /// <summary>
    /// Constructor para excepción de permisos insuficientes con permiso requerido específico
    /// </summary>
    /// <param name="resource">Recurso al que se intenta acceder</param>
    /// <param name="action">Acción que se intenta realizar</param>
    /// <param name="requiredPermission">Permiso específico requerido</param>
    public InsufficientPermissionsException(string resource, string action, string requiredPermission)
        : base("INSUFFICIENT_PERMISSIONS",
               $"Insufficient permissions to {action} {resource}. Required permission: {requiredPermission}",
               new { Resource = resource, Action = action, RequiredPermission = requiredPermission })
    {
    }

    /// <summary>
    /// Constructor para excepción de permisos insuficientes con múltiples permisos requeridos
    /// </summary>
    /// <param name="resource">Recurso al que se intenta acceder</param>
    /// <param name="action">Acción que se intenta realizar</param>
    /// <param name="requiredPermissions">Lista de permisos requeridos</param>
    public InsufficientPermissionsException(string resource, string action, IEnumerable<string> requiredPermissions)
        : base("INSUFFICIENT_PERMISSIONS",
               $"Insufficient permissions to {action} {resource}. Required permissions: {string.Join(", ", requiredPermissions)}",
               new { Resource = resource, Action = action, RequiredPermissions = requiredPermissions })
    {
    }
}