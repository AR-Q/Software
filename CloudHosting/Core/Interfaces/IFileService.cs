namespace CloudHosting.Core.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveAndExtractZipAsync(IFormFile file, int userId);
        void CleanupBuildContext(string buildPath);
    }
}