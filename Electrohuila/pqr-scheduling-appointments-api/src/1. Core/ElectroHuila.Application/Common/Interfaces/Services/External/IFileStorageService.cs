using ElectroHuila.Application.Common.Models;

namespace ElectroHuila.Application.Common.Interfaces.Services.External;

/// <summary>
/// Servicio para almacenamiento y gestión de archivos
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Sube un archivo al almacenamiento
    /// </summary>
    /// <param name="fileStream">Stream del archivo a subir</param>
    /// <param name="fileName">Nombre del archivo</param>
    /// <param name="contentType">Tipo de contenido del archivo</param>
    /// <returns>Resultado con la URL del archivo subido</returns>
    Task<Result<string>> UploadFileAsync(Stream fileStream, string fileName, string contentType);

    /// <summary>
    /// Descarga un archivo del almacenamiento
    /// </summary>
    /// <param name="fileName">Nombre del archivo a descargar</param>
    /// <returns>Resultado con el stream del archivo</returns>
    Task<Result<Stream>> DownloadFileAsync(string fileName);

    /// <summary>
    /// Elimina un archivo del almacenamiento
    /// </summary>
    /// <param name="fileName">Nombre del archivo a eliminar</param>
    /// <returns>Resultado indicando si la eliminación fue exitosa</returns>
    Task<Result<bool>> DeleteFileAsync(string fileName);

    /// <summary>
    /// Obtiene la URL pública de un archivo almacenado
    /// </summary>
    /// <param name="fileName">Nombre del archivo</param>
    /// <returns>Resultado con la URL del archivo</returns>
    Task<Result<string>> GetFileUrlAsync(string fileName);
}
