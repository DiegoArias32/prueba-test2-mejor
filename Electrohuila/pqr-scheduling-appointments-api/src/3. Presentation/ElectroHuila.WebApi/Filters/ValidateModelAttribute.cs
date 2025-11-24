using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ElectroHuila.WebApi.Filters;

/// <summary>
/// Filtro de acción para validar automáticamente el modelo de entrada.
/// Intercepta las acciones del controlador y valida el ModelState antes de la ejecución.
/// Retorna errores de validación estructurados en caso de modelo inválido.
/// </summary>
public class ValidateModelAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Se ejecuta antes de la acción del controlador.
    /// Valida el ModelState y retorna errores de validación si el modelo es inválido.
    /// </summary>
    /// <param name="context">Contexto de ejecución de la acción que contiene el ModelState</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            // Crea una respuesta estructurada con los errores de validación
            var errorResponse = new
            {
                message = "Validation failed",
                errors = errors
            };

            // Retorna BadRequest con los detalles de validación
            context.Result = new BadRequestObjectResult(errorResponse);
        }
    }
}