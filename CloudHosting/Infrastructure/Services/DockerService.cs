using Docker.DotNet;
using Docker.DotNet.Models;
using CloudHosting.Core.Interfaces;
using ICSharpCode.SharpZipLib.Tar;
using System.Text;
using CloudHosting.Core.Entities;

namespace CloudHosting.Infrastructure.Services 
{
    public class DockerService : IDockerService, IDisposable
    {
        private readonly DockerClient _client;
        private readonly ILogger<DockerService> _logger;
        private bool _disposed;

        public DockerService(IConfiguration configuration, ILogger<DockerService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            var dockerConnection = configuration["Docker:ConnectionString"] ?? "npipe://./pipe/docker_engine";
            _logger.LogInformation("Initializing Docker service with connection: {Connection}", dockerConnection);
            
            _client = new DockerClientConfiguration(new Uri(dockerConnection)).CreateClient();
        }

        public async Task<string> BuildImageAsync(string buildContextDir, string imageName)
        {
            ArgumentNullException.ThrowIfNull(buildContextDir);
            ArgumentNullException.ThrowIfNull(imageName);

            try
            {
                _logger.LogInformation("Building Docker image {ImageName} from context {Context}", imageName, buildContextDir);
                
                var tarStream = await CreateTarStreamFromDirectory(buildContextDir);
                await _client.Images.BuildImageFromDockerfileAsync(new ImageBuildParameters
                {
                    Dockerfile = "Dockerfile",
                    Tags = new[] { imageName },
                    NoCache = false
                }, tarStream, null, null, default);

                _logger.LogInformation("Successfully built Docker image {ImageName}", imageName);
                return imageName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build Docker image {ImageName} from {Context}", imageName, buildContextDir);
                throw new DockerOperationException($"Failed to build image: {ex.Message}", ex);
            }
        }

        public async Task<string> RunContainerAsync(string imageName, string containerName, CloudPlan plan)
        {
            ArgumentNullException.ThrowIfNull(imageName);
            ArgumentNullException.ThrowIfNull(containerName);
            ArgumentNullException.ThrowIfNull(plan);

            try
            {
                _logger.LogInformation("Creating container {ContainerName} from image {ImageName} with plan {PlanName}", 
                    containerName, imageName, plan.Name);
                
                var createResponse = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
                {
                    Image = imageName,
                    Name = containerName,
                    HostConfig = new HostConfig
                    {
                        Memory = plan.MaxMemoryMB * 1024L * 1024L,
                        CpusetCpus = $"0-{plan.MaxCpuCores - 1}",
                        RestartPolicy = new RestartPolicy 
                        { 
                            Name = RestartPolicyKind.OnFailure, 
                            MaximumRetryCount = 3 
                        },
                        AutoRemove = true
                    },
                    Env = new[] { $"MAX_MEMORY={plan.MaxMemoryMB}MB", $"CPU_CORES={plan.MaxCpuCores}" }
                });

                _logger.LogInformation("Starting container {ContainerId}", createResponse.ID);
                await _client.Containers.StartContainerAsync(createResponse.ID, new ContainerStartParameters());
                
                _logger.LogInformation("Container {ContainerId} started successfully", createResponse.ID);
                return createResponse.ID;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to run container {ContainerName} from image {ImageName}", containerName, imageName);
                throw new DockerOperationException($"Failed to run container: {ex.Message}", ex);
            }
        }

        public async Task<bool> StopContainerAsync(string containerId)
        {
            ArgumentNullException.ThrowIfNull(containerId);

            try
            {
                _logger.LogInformation("Stopping container {ContainerId}", containerId);
                
                await _client.Containers.StopContainerAsync(containerId, new ContainerStopParameters
                {
                    WaitBeforeKillSeconds = 10
                });
                
                _logger.LogInformation("Container {ContainerId} stopped successfully", containerId);
                return true;
            }
            catch (DockerContainerNotFoundException)
            {
                _logger.LogWarning("Container {ContainerId} not found when attempting to stop", containerId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to stop container {ContainerId}", containerId);
                throw new DockerOperationException($"Failed to stop container: {ex.Message}", ex);
            }
        }

        public async Task<string> GetContainerLogsAsync(string containerId)
        {
            ArgumentNullException.ThrowIfNull(containerId);

            try
            {
                _logger.LogInformation("Retrieving logs for container {ContainerId}", containerId);
                
                var parameters = new ContainerLogsParameters
                {
                    ShowStdout = true,
                    ShowStderr = true,
                    Follow = false,
                    Tail = "101"
                };

                // Using the updated API call
                using var logs = _client.Containers.GetContainerLogsAsync(containerId, true, parameters);
                using var reader = await logs;  
                var logContent = await reader.ReadOutputToEndAsync(CancellationToken.None);
                
                _logger.LogDebug("Retrieved {Length} bytes of logs for container {ContainerId}", logContent.stdout.Length, containerId);
                return logContent.stdout;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get logs for container {ContainerId}", containerId);
                throw new DockerOperationException($"Failed to get container logs: {ex.Message}", ex);
            }
        }

        public async Task<Model.ResourceInfo> GetResourceInfoAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving Docker resource information");
                
                var info = await _client.System.GetSystemInfoAsync();
                
                // detailed resource information
                var containers = await _client.Containers.ListContainersAsync(new ContainersListParameters
                {
                    All = false, // only running containers
                });
                
                // CPU and memory usage
                double totalCpuPercent = 0;
                long totalMemoryUsage = 0;
                
                foreach (var container in containers)
                {
                    try 
                    {

                        var statsStream = await _client.Containers.GetContainerStatsAsync(container.ID, new ContainerStatsParameters{},CancellationToken.None);
                        
                        using (var reader = new StreamReader(statsStream))
                        {
                            var statsJson = await reader.ReadToEndAsync();
                            totalMemoryUsage += 100 * 1024 * 1024; // Placeholder
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not get stats for container {ContainerId}", container.ID);
                    }
                }
                
                var resourceInfo = new Model.ResourceInfo
                {
                    AvailableRam = $"{info.MemTotal / (1024 * 1024 * 1024)}GB",
                    InUseRam = $"{totalMemoryUsage / (1024 * 1024)}MB", 
                    CpuUtilization = $"{info.NCPU} cores",
                    StorageUsed = await GetStorageUsedAlternativeAsync()
                };
                
                _logger.LogInformation("Retrieved resource info: RAM={AvailableRam}, CPU={CpuUtilization}, Storage={StorageUsed}", 
                    resourceInfo.AvailableRam, resourceInfo.CpuUtilization, resourceInfo.StorageUsed);
                    
                return resourceInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Docker resource information");
                throw new DockerOperationException($"Failed to get resource info: {ex.Message}", ex);
            }
        }

        private async Task<string> GetStorageUsedAlternativeAsync()
        {
            try
            {
                var images = await _client.Images.ListImagesAsync(new ImagesListParameters { All = true });
                long totalSize = 0;
                foreach (var image in images)
                {
                    totalSize += image.Size;
                }
                return $"{(double)totalSize / (1024 * 1024 * 1024):F2}GB";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not get Docker storage information");
                return "N/A";
            }
        }

        private async Task<Stream> CreateTarStreamFromDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                _logger.LogError("Build context directory not found: {Directory}", directory);
                throw new DirectoryNotFoundException($"Build context directory not found: {directory}");
            }

            _logger.LogDebug("Creating tar archive from directory: {Directory}", directory);
            var memoryStream = new MemoryStream();
            
            try
            {
                using (var tarArchive = new TarOutputStream(memoryStream, Encoding.UTF8))
                {
                    tarArchive.IsStreamOwner = false;
                    
                    // all files in the directory added to the tar
                    foreach (var filePath in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = Path.GetRelativePath(directory, filePath).Replace('\\', '/');
                        _logger.LogTrace("Adding file to tar: {RelativePath}", relativePath);
                        
                        var entry = TarEntry.CreateEntryFromFile(filePath);
                        entry.Name = relativePath;
                        
                        tarArchive.PutNextEntry(entry);
                        using (var fileStream = File.OpenRead(filePath))
                        {
                            await fileStream.CopyToAsync(tarArchive);
                        }
                        tarArchive.CloseEntry();
                    }
                    
                    await tarArchive.FlushAsync();
                }
                
                memoryStream.Position = 0;
                _logger.LogDebug("Successfully created tar archive of {Size} bytes", memoryStream.Length);
                return memoryStream;
            }
            catch (Exception ex)
            {
                memoryStream.Dispose();
                _logger.LogError(ex, "Error creating tar stream from directory: {Directory}", directory);
                throw new DockerOperationException("Failed to create tar archive for Docker build context", ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _logger?.LogDebug("Disposing Docker client");
                _client?.Dispose();
            }

            _disposed = true;
        }
    }

    public class DockerOperationException : Exception
    {
        public DockerOperationException(string message, Exception? innerException = null) 
            : base(message, innerException)
        {
        }
    }
}