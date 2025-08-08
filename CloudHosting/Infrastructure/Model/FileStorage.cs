namespace CloudHosting.Infrastructure.Config
{
    public class FileStorageOptions
    {
        public const string SectionName = "FileStorage";
        
        public string BasePath { get; set; } = string.Empty;
        public string TempPath { get; set; } = string.Empty;
        public long MaxFileSizeBytes { get; set; } = 100 * 1024 * 1024; // 100MB default
        public string[] AllowedExtensions { get; set; } = [".zip"];
    }
}