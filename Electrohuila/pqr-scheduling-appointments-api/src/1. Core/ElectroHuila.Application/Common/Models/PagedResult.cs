namespace ElectroHuila.Application.Common.Models;

/// <summary>
/// Modelo genérico para resultados paginados
/// </summary>
/// <typeparam name="T">Tipo de entidad a paginar</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Lista de elementos de la página actual
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Total de registros en la base de datos
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Número de página actual (basado en 1)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Cantidad de elementos por página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de páginas disponibles
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Indica si existe una página anterior
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indica si existe una página siguiente
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
