namespace CloudHosting.Infrastructure.Config
{
    public class FileStorageOptions
    {
        public const string BasePath = "C:\\CloudHost\\Base\\";
        public const string TempPath = "C:\\CloudHost\\Temp\\";
        public const long MaxFileSizeBytes = 100 * 1024 * 1024; // 100MB default
        public const string AllowedExtensions = ".zip";
    }
}