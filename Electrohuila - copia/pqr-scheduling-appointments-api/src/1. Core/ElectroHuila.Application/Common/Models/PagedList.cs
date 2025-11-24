namespace ElectroHuila.Application.Common.Models;

/// <summary>
/// Representa una lista paginada de elementos con metadatos de paginación
/// </summary>
/// <typeparam name="T">Tipo de elementos en la lista</typeparam>
public class PagedList<T>
{
    /// <summary>
    /// Colección de solo lectura de los elementos de la página actual
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// Número total de elementos en todas las páginas
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Número de la página actual (base 1)
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Cantidad de elementos por página
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Número total de páginas calculadas
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// Indica si existe una página anterior
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indica si existe una página siguiente
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Constructor que inicializa una lista paginada con sus metadatos
    /// </summary>
    /// <param name="items">Elementos de la página actual</param>
    /// <param name="totalCount">Total de elementos en todas las páginas</param>
    /// <param name="pageNumber">Número de página actual</param>
    /// <param name="pageSize">Cantidad de elementos por página</param>
    public PagedList(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items.ToList().AsReadOnly();
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    /// <summary>
    /// Crea una lista paginada a partir de una fuente de datos aplicando paginación
    /// </summary>
    /// <param name="source">Fuente de datos completa</param>
    /// <param name="pageNumber">Número de página a obtener</param>
    /// <param name="pageSize">Cantidad de elementos por página</param>
    /// <returns>Lista paginada con los elementos de la página solicitada</returns>
    public static PagedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var items = source.ToList();
        var totalCount = items.Count;
        var pagedItems = items
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return new PagedList<T>(pagedItems, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Crea una lista paginada vacía
    /// </summary>
    /// <param name="pageNumber">Número de página</param>
    /// <param name="pageSize">Cantidad de elementos por página</param>
    /// <returns>Lista paginada sin elementos</returns>
    public static PagedList<T> Empty(int pageNumber, int pageSize) =>
        new(Enumerable.Empty<T>(), 0, pageNumber, pageSize);
}