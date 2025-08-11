namespace CloudHosting.Core.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveAndExtractZipAsync(IFormFile file, string imageName, string userId);
        void CleanupBuildContext(string buildPath);
        Task<IEnumerable<string>> GetUserDirectoriesAsync(string userId);
        Task<IEnumerable<string?>> GetUserDockerImagesAsync(string userId);
    }
}