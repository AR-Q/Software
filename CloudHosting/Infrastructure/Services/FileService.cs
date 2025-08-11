using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Config;
using System.IO.Compression;

namespace CloudHosting.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly IIamService _iamService;

        public FileService(ILogger<FileService> logger, IIamService iamService)
        {
            _logger = logger;
            _iamService = iamService;
        }

        public async Task<string> SaveAndExtractZipAsync(IFormFile file, string newImageName, string userId)
        {

            if (string.IsNullOrEmpty((string)userId))
            {
                throw new UnauthorizedAccessException("Invalid user id");
            }

            if (file.Length > FileStorageOptions.MaxFileSizeBytes)
            {
                throw new InvalidOperationException($"File size exceeds maximum allowed size of {FileStorageOptions.MaxFileSizeBytes / 1024 / 1024}MB");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!FileStorageOptions.AllowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException($"File type {extension} is not allowed");
            }

            var userTempPath = Path.Combine(Path.Combine(FileStorageOptions.TempPath, userId), newImageName);
            var userBuildPath = Path.Combine(Path.Combine(FileStorageOptions.BasePath, userId), newImageName);

            Directory.CreateDirectory(userTempPath);
            Directory.CreateDirectory(userBuildPath);

            try
            {
                // Save zip to temp
                var zipPath = Path.Combine(userTempPath, "upload.zip");

                using (var stream = new FileStream(zipPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Verify and extract
                bool hasDockerfile = false;

                using (var archive = ZipFile.OpenRead(zipPath))
                {
                    // Check for Dockerfile
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName.Equals("Dockerfile", StringComparison.OrdinalIgnoreCase))
                        {
                            hasDockerfile = true;
                            break;
                        }
                    }

                    if (!hasDockerfile)
                    {
                        throw new InvalidDataException("ZIP file must contain a Dockerfile");
                    }
                }

                // Extract to build path
                ZipFile.ExtractToDirectory(zipPath, userBuildPath, true);

                return userBuildPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process upload for user {UserId}", userId);

                CleanupBuildContext(userBuildPath);
                CleanupBuildContext(userTempPath);

                throw;
            }
            finally
            {
                CleanupBuildContext(userTempPath);
            }
        }

        public async Task<IEnumerable<string>> GetUserDirectoriesAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("Invalid userId");
                }

                var userBasePath = Path.Combine(FileStorageOptions.BasePath, userId);

                if (!Directory.Exists(userBasePath))
                {
                    return Enumerable.Empty<string>();
                }

                return Directory.GetDirectories(userBasePath)
                    .Select(path => Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get directories for user");
                throw;
            }
        }

        public async Task<IEnumerable<string?>> GetUserDockerImagesAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("Invalid user id");
                }

                var userBasePath = Path.Combine(FileStorageOptions.BasePath, userId);
                
                if (!Directory.Exists(userBasePath))
                {
                    return Enumerable.Empty<string>();
                }

                // all directories that contain a Dockerfile

                IEnumerable<string> dockerDirectoryNames = ["..."];

                return dockerDirectoryNames.Concat(Directory.GetDirectories(userBasePath)
                    .Where(dir => File.Exists(Path.Combine(dir, "Dockerfile")))
                    .Select(x => Path.GetDirectoryName(x)));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get docker directories for user");
                throw;
            }
        }

        public void CleanupBuildContext(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cleanup path: {Path}", path);
            }
        }
    }
}