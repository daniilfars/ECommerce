namespace Catalog.Application.Interfaces;

public interface IImageStorageService
{
    Task<string> UploadAsync(string objectName, Stream data, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(string objectName, CancellationToken cancellationToken = default);
    string GetPublicUrl(string objectName);
    string GetObjectNameFromUrl(string url);
}