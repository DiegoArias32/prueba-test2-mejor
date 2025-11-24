using FluentValidation;
using MediatR;

namespace ElectroHuila.Application.Common.Behaviors;

/// <summary>
/// Comportamiento de pipeline de MediatR que valida las solicitudes antes de ser procesadas
/// </summary>
/// <typeparam name="TRequest">Tipo de solicitud que implementa IRequest</typeparam>
/// <typeparam name="TResponse">Tipo de respuesta de la solicitud</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Constructor que inyecta los validadores registrados para el tipo de solicitud
    /// </summary>
    /// <param name="validators">Colección de validadores de FluentValidation para la solicitud</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Ejecuta la validación de la solicitud antes de continuar con el pipeline
    /// </summary>
    /// <param name="request">Solicitud a validar</param>
    /// <param name="next">Delegado para continuar con el siguiente comportamiento en el pipeline</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Respuesta del siguiente handler en el pipeline</returns>
    /// <exception cref="ValidationException">Se lanza cuando la validación falla con los errores encontrados</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Solo ejecutar validación si hay validadores registrados
        if (_validators.Any())
        {
            // Crear contexto de validación
            var context = new ValidationContext<TRequest>(request);

            // Ejecutar todos los validadores en paralelo
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Recolectar todos los errores de validación
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            // Si hay errores, lanzar excepción con todos los fallos
            if (failures.Count != 0)
                throw new ValidationException(failures);
        }

        // Continuar con el siguiente comportamiento en el pipeline
        return await next();
    }
}