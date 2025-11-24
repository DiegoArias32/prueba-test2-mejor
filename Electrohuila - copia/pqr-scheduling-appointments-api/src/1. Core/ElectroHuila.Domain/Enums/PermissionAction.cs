namespace ElectroHuila.Domain.Enums;

/// <summary>
/// Enumeración de acciones de permisos usando flags para combinaciones
/// </summary>
[Flags]
public enum PermissionAction
{
    /// <summary>
    /// Permiso de lectura
    /// </summary>
    Read = 1,

    /// <summary>
    /// Permiso de creación
    /// </summary>
    Create = 2,

    /// <summary>
    /// Permiso de actualización
    /// </summary>
    Update = 4,

    /// <summary>
    /// Permiso de eliminación
    /// </summary>
    Delete = 8,

    /// <summary>
    /// Permiso de exportación
    /// </summary>
    Export = 16,

    /// <summary>
    /// Permiso de importación
    /// </summary>
    Import = 32,

    /// <summary>
    /// Permiso de aprobación
    /// </summary>
    Approve = 64,

    /// <summary>
    /// Permiso de rechazo
    /// </summary>
    Reject = 128
}

/// <summary>
/// Métodos de extensión para el enum PermissionAction
/// </summary>
public static class PermissionActionExtensions
{
    /// <summary>
    /// Convierte la acción de permiso a su representación en cadena para mostrar
    /// </summary>
    /// <param name="action">Acción de permiso</param>
    /// <returns>Representación en cadena de la acción</returns>
    public static string ToDisplayString(this PermissionAction action)
    {
        return action switch
        {
            PermissionAction.Read => "Leer",
            PermissionAction.Create => "Crear",
            PermissionAction.Update => "Actualizar",
            PermissionAction.Delete => "Eliminar",
            PermissionAction.Export => "Exportar",
            PermissionAction.Import => "Importar",
            PermissionAction.Approve => "Aprobar",
            PermissionAction.Reject => "Rechazar",
            _ => action.ToString()
        };
    }

    /// <summary>
    /// Verifica si un conjunto de permisos contiene una acción específica
    /// </summary>
    /// <param name="permissions">Conjunto de permisos como entero</param>
    /// <param name="action">Acción a verificar</param>
    /// <returns>True si el permiso está presente, false en caso contrario</returns>
    public static bool HasPermission(this int permissions, PermissionAction action)
    {
        return (permissions & (int)action) == (int)action;
    }

    /// <summary>
    /// Agrega una acción de permiso al conjunto de permisos
    /// </summary>
    /// <param name="permissions">Conjunto de permisos actual</param>
    /// <param name="action">Acción a agregar</param>
    /// <returns>Nuevo conjunto de permisos con la acción agregada</returns>
    public static int AddPermission(this int permissions, PermissionAction action)
    {
        return permissions | (int)action;
    }

    /// <summary>
    /// Remueve una acción de permiso del conjunto de permisos
    /// </summary>
    /// <param name="permissions">Conjunto de permisos actual</param>
    /// <param name="action">Acción a remover</param>
    /// <returns>Nuevo conjunto de permisos sin la acción especificada</returns>
    public static int RemovePermission(this int permissions, PermissionAction action)
    {
        return permissions & ~(int)action;
    }

    /// <summary>
    /// Obtiene todas las acciones de permiso otorgadas en un conjunto de permisos
    /// </summary>
    /// <param name="permissions">Conjunto de permisos</param>
    /// <returns>Enumeración de acciones de permiso otorgadas</returns>
    public static IEnumerable<PermissionAction> GetGrantedPermissions(this int permissions)
    {
        var actions = Enum.GetValues<PermissionAction>();
        return actions.Where(action => permissions.HasPermission(action));
    }

    /// <summary>
    /// Obtiene un conjunto con todos los permisos disponibles
    /// </summary>
    /// <returns>Conjunto con todos los permisos habilitados</returns>
    public static int GetFullPermissions()
    {
        return Enum.GetValues<PermissionAction>().Aggregate(0, (current, action) => current | (int)action);
    }

    /// <summary>
    /// Obtiene un conjunto de permisos de solo lectura (leer y exportar)
    /// </summary>
    /// <returns>Conjunto de permisos de solo lectura</returns>
    public static int GetReadOnlyPermissions()
    {
        return (int)PermissionAction.Read | (int)PermissionAction.Export;
    }

    /// <summary>
    /// Obtiene un conjunto de permisos básicos (leer, crear y actualizar)
    /// </summary>
    /// <returns>Conjunto de permisos básicos</returns>
    public static int GetBasicPermissions()
    {
        return (int)PermissionAction.Read | (int)PermissionAction.Create | (int)PermissionAction.Update;
    }
}