namespace CloudHosting.Core.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string directory);
        Task<Stream> GetFileAsync(string filePath);
        Task DeleteFileAsync(string filePath);
        string GetStoragePath(int userId, string fileType);
    }
}