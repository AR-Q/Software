namespace CloudHosting.Core.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveAndExtractZipAsync(IFormFile file, string imageName, string userId);
        void CleanupBuildContext(string buildPath);
    }
}