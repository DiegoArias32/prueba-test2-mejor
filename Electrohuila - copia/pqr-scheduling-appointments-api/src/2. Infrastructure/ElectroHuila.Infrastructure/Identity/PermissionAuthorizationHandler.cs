using Microsoft.AspNetCore.Authorization;
using ElectroHuila.Application.Common.Interfaces.Services.Common;
using System.Security.Claims;

namespace ElectroHuila.Infrastructure.Identity;

/// <summary>
/// Requisito de autorización que especifica un permiso requerido para acceder a un recurso.
/// Implementa <see cref="IAuthorizationRequirement"/> para ser utilizado en el sistema de autorización de ASP.NET Core.
/// </summary>
/// <remarks>
/// Esta clase encapsula un permiso específico que debe verificarse durante el proceso de autorización.
/// Se utiliza en conjunto con <see cref="PermissionAuthorizationHandler"/> para evaluar si un usuario
/// tiene el permiso necesario para realizar una acción específica.
/// </remarks>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Obtiene el nombre del permiso requerido.
    /// </summary>
    /// <value>
    /// El nombre del permiso como string (ej: "users.create", "appointments.read").
    /// </value>
    public string Permission { get; }

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="PermissionRequirement"/>.
    /// </summary>
    /// <param name="permission">El nombre del permiso requerido para la autorización.</param>
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

/// <summary>
/// Manejador de autorización que evalúa si un usuario tiene un permiso específico.
/// Hereda de <see cref="AuthorizationHandler{T}"/> para implementar la lógica de autorización basada en permisos.
/// </summary>
/// <remarks>
/// Este handler examina los claims del usuario actual para verificar si posee el permiso requerido.
/// Busca claims de tipo "permission" en el contexto del usuario autenticado y los compara
/// con el permiso especificado en el requisito de autorización.
/// </remarks>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <summary>
    /// Evalúa si el usuario actual cumple con el requisito de permiso especificado.
    /// </summary>
    /// <param name="context">Contexto de autorización que contiene información del usuario y recurso.</param>
    /// <param name="requirement">Requisito de permiso que debe verificarse.</param>
    /// <returns>
    /// Task completado. Si el usuario tiene el permiso, se marca el requisito como exitoso
    /// llamando a <see cref="AuthorizationHandlerContext.Succeed"/>.
    /// </returns>
    /// <remarks>
    /// El método:
    /// 1. Extrae todos los claims de tipo "permission" del usuario actual
    /// 2. Verifica si alguno de estos permisos coincide con el requerido
    /// 3. Si encuentra una coincidencia, marca el requisito como exitoso
    /// 4. Si no encuentra coincidencia, no hace nada (fallo implícito)
    /// </remarks>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permissions = context.User.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}