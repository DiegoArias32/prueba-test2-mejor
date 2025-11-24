using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ElectroHuila.WebApi.Filters;

/// <summary>
/// Filtro de operación de Swagger que agrega automáticamente el parámetro 'database' 
/// a todos los endpoints de la API con una lista desplegable de proveedores.
/// </summary>
public class DatabaseParameterOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Inicializar la lista de parámetros si no existe
        operation.Parameters ??= new List<OpenApiParameter>();

        // Verificar si el parámetro 'database' ya existe para evitar duplicados
        if (operation.Parameters.Any(p => p.Name == "database"))
        {
            return; // Ya existe, no lo agregamos de nuevo
        }

        // Agregar el parámetro 'database' como enum con valores predefinidos
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "database",
            In = ParameterLocation.Query,
            Description = "Seleccionar base de datos para esta operación",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Enum = new List<IOpenApiAny>
                {
                    new OpenApiString("oracle"),
                    new OpenApiString("sqlserver"),
                    new OpenApiString("postgresql")
                },
                Default = new OpenApiString("oracle")
            }
        });
    }
}
