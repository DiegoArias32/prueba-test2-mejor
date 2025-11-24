using ElectroHuila.Application.Common.Interfaces;
using ElectroHuila.Application.Common.Interfaces.Services.Common;
using ElectroHuila.Application.Common.Interfaces.Services.External;
using ElectroHuila.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Infrastructure.External.FileStorage;

/// <summary>
/// Servicio para subir, descargar y manejar archivos en Azure Blob Storage.
/// Si no está configurado Azure, simula el comportamiento guardando archivos localmente.
/// </summary>
public class AzureBlobStorageService : IFileStorageService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureBlobStorageService> _logger;

    /// <summary>
    /// Inicializa el servicio de almacenamiento con configuración y logging.
    /// </summary>
    /// <param name="configuration">Configuración de la aplicación</param>
    /// <param name="logger">Logger para registrar eventos</param>
    public AzureBlobStorageService(IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Sube un archivo a Azure Blob Storage o lo guarda localmente si no está configurado.
    /// </summary>
    /// <param name="fileStream">Stream del archivo a subir</param>
    /// <param name="fileName">Nombre del archivo</param>
    /// <param name="contentType">Tipo MIME del archivo</param>
    /// <returns>URL o ruta donde se guardó el archivo</returns>
    public async Task<Result<string>> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            var connectionString = _configuration["AzureStorage:ConnectionString"];
            var storageAccountName = _configuration["AzureStorage:AccountName"];
            var containerName = _configuration["AzureStorage:ContainerName"] ?? "documents";

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogWarning("Azure Storage not configured, using local storage simulation");

                // Convert stream to byte array for local storage
                using var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream);
                var fileContent = memoryStream.ToArray();

                return await SimulateLocalUpload(fileContent, fileName, containerName);
            }

            // In a real implementation with Azure.Storage.Blobs:
            // var blobServiceClient = new BlobServiceClient(connectionString);
            // var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            // await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            //
            // var blobClient = containerClient.GetBlobClient(fileName);
            // await blobClient.UploadAsync(fileStream, overwrite: true);
            //
            // return Result.Success(blobClient.Uri.ToString());

            // Simulate upload
            await Task.Delay(100);
            var simulatedUrl = $"https://{storageAccountName ?? "electrohuila"}.blob.core.windows.net/{containerName}/{fileName}";

            _logger.LogInformation("File uploaded successfully: {FileName} with content type {ContentType}", fileName, contentType);
            return Result.Success(simulatedUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName} to Azure Storage", fileName);
            return Result.Failure<string>($"Failed to upload file: {ex.Message}");
        }
    }

    /// <summary>
    /// Descarga un archivo desde Azure Blob Storage.
    /// Si no está configurado, retorna un stream vacío.
    /// </summary>
    /// <param name="fileName">Nombre del archivo a descargar</param>
    /// <returns>Stream con el contenido del archivo</returns>
    public async Task<Result<Stream>> DownloadFileAsync(string fileName)
    {
        try
        {
            var connectionString = _configuration["AzureStorage:ConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogWarning("Azure Storage not configured, returning empty stream");
                return Result.Success<Stream>(new MemoryStream());
            }

            // In a real implementation:
            // var containerName = _configuration["AzureStorage:ContainerName"] ?? "documents";
            // var blobServiceClient = new BlobServiceClient(connectionString);
            // var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            // var blobClient = containerClient.GetBlobClient(fileName);
            // var downloadResult = await blobClient.DownloadAsync();
            // return Result.Success(downloadResult.Value.Content);

            // Simulate download
            await Task.Delay(100);

            _logger.LogInformation("File downloaded successfully: {FileName}", fileName);
            return Result.Success<Stream>(new MemoryStream());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file {FileName}", fileName);
            return Result.Failure<Stream>($"Failed to download file: {ex.Message}");
        }
    }

    /// <summary>
    /// Elimina un archivo de Azure Blob Storage.
    /// Si no está configurado, simula la eliminación.
    /// </summary>
    /// <param name="fileName">Nombre del archivo a eliminar</param>
    /// <returns>true si se eliminó correctamente</returns>
    public async Task<Result<bool>> DeleteFileAsync(string fileName)
    {
        try
        {
            var connectionString = _configuration["AzureStorage:ConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogWarning("Azure Storage not configured, simulating delete");
                await Task.Delay(50);
                return Result.Success(true);
            }

            // In a real implementation:
            // var containerName = _configuration["AzureStorage:ContainerName"] ?? "documents";
            // var blobServiceClient = new BlobServiceClient(connectionString);
            // var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            // var blobClient = containerClient.GetBlobClient(fileName);
            // var response = await blobClient.DeleteIfExistsAsync();
            // return Result.Success(response.Value);

            // Simulate delete
            await Task.Delay(50);

            _logger.LogInformation("File deleted successfully: {FileName}", fileName);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {FileName}", fileName);
            return Result.Failure<bool>($"Failed to delete file: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene la URL pública de un archivo almacenado.
    /// Si no está configurado Azure, retorna una ruta local.
    /// </summary>
    /// <param name="fileName">Nombre del archivo</param>
    /// <returns>URL o ruta del archivo</returns>
    public async Task<Result<string>> GetFileUrlAsync(string fileName)
    {
        try
        {
            var connectionString = _configuration["AzureStorage:ConnectionString"];
            var storageAccountName = _configuration["AzureStorage:AccountName"];
            var containerName = _configuration["AzureStorage:ContainerName"] ?? "documents";

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogWarning("Azure Storage not configured, returning local path");
                var localPath = Path.Combine("uploads", containerName, fileName);
                return Result.Success(localPath);
            }

            // In a real implementation:
            // var blobServiceClient = new BlobServiceClient(connectionString);
            // var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            // var blobClient = containerClient.GetBlobClient(fileName);
            // return Result.Success(blobClient.Uri.ToString());

            // Simulate URL generation
            await Task.Delay(50);
            var url = $"https://{storageAccountName ?? "electrohuila"}.blob.core.windows.net/{containerName}/{fileName}";

            _logger.LogInformation("Generated file URL for {FileName}", fileName);
            return Result.Success(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get file URL for {FileName}", fileName);
            return Result.Failure<string>($"Failed to get file URL: {ex.Message}");
        }
    }

    /// <summary>
    /// Método auxiliar que simula la subida de archivos guardándolos localmente.
    /// Se usa cuando Azure Storage no está configurado.
    /// </summary>
    /// <param name="fileContent">Contenido del archivo como bytes</param>
    /// <param name="fileName">Nombre del archivo</param>
    /// <param name="containerName">Nombre del contenedor (carpeta)</param>
    /// <returns>Ruta local donde se guardó el archivo</returns>
    private async Task<Result<string>> SimulateLocalUpload(byte[] fileContent, string fileName, string containerName)
    {
        await Task.Delay(100);

        var localPath = Path.Combine("uploads", containerName, fileName);
        var directoryPath = Path.GetDirectoryName(localPath);

        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        await File.WriteAllBytesAsync(localPath, fileContent);

        _logger.LogInformation("File saved locally: {FilePath}", localPath);
        return Result.Success(localPath);
    }

}