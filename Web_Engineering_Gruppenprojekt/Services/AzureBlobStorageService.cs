using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace Web_Engineering_Gruppenprojekt.Services;

public class AzureBlobStorageService(IConfiguration config) : IFileStorageService
{
    private BlobContainerClient GetContainer()
    {
        var connStr = config.GetConnectionString("AzureBlobStorage")!;
        var container = config["AzureBlobStorage:ContainerName"] ?? "slides";
        return new BlobContainerClient(connStr, container);
    }

    private static string GetBlobName(string filePath) => Path.GetFileName(new Uri(filePath).LocalPath);

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
        await container.GetBlobClient(GetBlobName(filePath)).DeleteIfExistsAsync();
    }

    public async Task<byte[]?> DownloadAsync(string filePath)
    {
        var blob = GetContainer().GetBlobClient(GetBlobName(filePath));
        if (!await blob.ExistsAsync()) return null;

        var content = await blob.DownloadContentAsync();
        return content.Value.Content.ToArray();
    }

    public string GetUrl(string filePath)
    {
        var blob = GetContainer().GetBlobClient(GetBlobName(filePath));
        if (!blob.CanGenerateSasUri) return filePath;

        var sasUri = blob.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));
        return sasUri.ToString();
    }
}
