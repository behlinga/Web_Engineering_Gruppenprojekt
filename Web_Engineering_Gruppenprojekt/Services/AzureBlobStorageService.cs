using Azure.Storage.Blobs;

namespace Web_Engineering_Gruppenprojekt.Services;

public class AzureBlobStorageService(IConfiguration config) : IFileStorageService
{
    private BlobContainerClient GetContainer()
    {
        var connStr = config["AzureBlobStorage:ConnectionString"]!;
        var container = config["AzureBlobStorage:ContainerName"] ?? "slides";
        return new BlobContainerClient(connStr, container);
    }

    public async Task<(string fileName, string filePath)> UploadAsync(IFormFile file)
    {
        var container = GetContainer();
        await container.CreateIfNotExistsAsync();

        var blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var blob = container.GetBlobClient(blobName);

        await using var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, overwrite: true);

        return (blobName, blob.Uri.ToString());
    }

    public async Task DeleteAsync(string filePath)
    {
        var container = GetContainer();
        var blobName = Path.GetFileName(new Uri(filePath).LocalPath);
        await container.GetBlobClient(blobName).DeleteIfExistsAsync();
    }

    public string GetUrl(string filePath) => filePath;
}
