namespace Web_Engineering_Gruppenprojekt.Services;

public interface IFileStorageService
{
    Task<(string fileName, string filePath)> UploadAsync(IFormFile file);
    Task DeleteAsync(string filePath);
    string GetUrl(string filePath);
}
