namespace Web_Engineering_Gruppenprojekt.Services;

public class LocalFileStorageService(IWebHostEnvironment env) : IFileStorageService
{
    private readonly string _uploadRoot = Path.Combine(env.WebRootPath, "uploads");

    public async Task<(string fileName, string filePath)> UploadAsync(IFormFile file)
    {
        Directory.CreateDirectory(_uploadRoot);
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(_uploadRoot, fileName);

        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream);

        return (fileName, $"/uploads/{fileName}");
    }

    public Task DeleteAsync(string filePath)
    {
        var fullPath = Path.Combine(env.WebRootPath, filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public async Task<byte[]?> DownloadAsync(string filePath)
    {
        var fullPath = Path.Combine(env.WebRootPath, filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        return File.Exists(fullPath) ? await File.ReadAllBytesAsync(fullPath) : null;
    }

    public string GetUrl(string filePath) => filePath;
}
