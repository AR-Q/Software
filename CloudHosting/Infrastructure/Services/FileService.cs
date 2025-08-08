using System.IO.Compression;
using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Config;
using Microsoft.Extensions.Options;

namespace CloudHosting.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly string _basePath;
        private readonly string _tempPath;
        private readonly ILogger<FileService> _logger;
        private readonly FileStorageOptions _options;

        public FileService(IOptions<FileStorageOptions> options, ILogger<FileService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _basePath = _options.BasePath;
            _tempPath = _options.TempPath;
            
            Directory.CreateDirectory(_basePath);
            Directory.CreateDirectory(_tempPath);
        }

        public string GetUserBasePath(int userId)
        {
            var path = Path.Combine(_basePath, userId.ToString());
            Directory.CreateDirectory(path);
            return path;
        }

        public string GetUserTempPath(int userId)
        {
            var path = Path.Combine(_tempPath, userId.ToString());
            Directory.CreateDirectory(path);
            return path;
        }

        public async Task<string> SaveAndExtractZipAsync(IFormFile file, int userId)
        {
            if (file.Length > _options.MaxFileSizeBytes)
            {
                throw new InvalidOperationException($"File size exceeds maximum allowed size of {_options.MaxFileSizeBytes / 1024 / 1024}MB");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_options.AllowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException($"File type {extension} is not allowed");
            }

            var buildId = Guid.NewGuid().ToString();
            var tempPath = Path.Combine(GetUserTempPath(userId), buildId);
            var buildPath = Path.Combine(GetUserBasePath(userId), buildId);

            try
            {
                Directory.CreateDirectory(tempPath);
                Directory.CreateDirectory(buildPath);

                // Save zip to temp
                var zipPath = Path.Combine(tempPath, "upload.zip");
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
                ZipFile.ExtractToDirectory(zipPath, buildPath, true);

                return buildPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process upload for user {UserId}", userId);
                CleanupBuildContext(buildPath);
                CleanupBuildContext(tempPath);
                throw;
            }
            finally
            {
                CleanupBuildContext(tempPath);
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