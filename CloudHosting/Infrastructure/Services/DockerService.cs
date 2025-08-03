using Docker.DotNet;
using Docker.DotNet.Models;
using CloudHosting.Infrastructure.Model;
using CloudHosting.Core.Interfaces;

namespace CloudHosting.Infrastructure.Services 
{
    public class DockerService : IDockerService, IDisposable
    {
        private readonly DockerClient _client;
        private bool _disposed;

        public DockerService()
        {
            _client = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
        }

        public async Task<string> BuildImageAsync(string buildContextDir, string imageName)
        {
            ArgumentNullException.ThrowIfNull(buildContextDir);
            ArgumentNullException.ThrowIfNull(imageName);

            try
            {
                var tarStream = await CreateTarStreamFromDirectory(buildContextDir);
                await _client.Images.BuildImageFromDockerfileAsync(new ImageBuildParameters
                {
                    Dockerfile = "Dockerfile",
                    Tags = new[] { imageName },
                    NoCache = false
                }, tarStream, null, null, default);

                return imageName;
            }
            catch (Exception ex)
            {
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

                await _client.Containers.StartContainerAsync(createResponse.ID, new ContainerStartParameters());
                return createResponse.ID;
            }
            catch (Exception ex)
            {
                throw new DockerOperationException($"Failed to run container: {ex.Message}", ex);
            }
        }

        public async Task<bool> StopContainerAsync(string containerId)
        {
            ArgumentNullException.ThrowIfNull(containerId);

            try
            {
                await _client.Containers.StopContainerAsync(containerId, new ContainerStopParameters
                {
                    WaitBeforeKillSeconds = 10
                });
                return true;
            }
            catch (DockerContainerNotFoundException)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw new DockerOperationException($"Failed to stop container: {ex.Message}", ex);
            }
        }

        public async Task<string> GetContainerLogsAsync(string containerId)
        {
            ArgumentNullException.ThrowIfNull(containerId);

            try
            {
                var parameters = new ContainerLogsParameters
                {
                    ShowStdout = true,
                    ShowStderr = true,
                    Follow = false,
                    Tail = "101"
                };

                using var logs = await _client.Containers.GetContainerLogsAsync(containerId, parameters);
                using var reader = new StreamReader(logs);
                return await reader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                throw new DockerOperationException($"Failed to get container logs: {ex.Message}", ex);
            }
        }

        public async Task<ResourceInfo> GetResourceInfoAsync()
        {
            try
            {
                var info = await _client.System.GetSystemInfoAsync();
                
                return new ResourceInfo
                {
                    AvailableRam = $"{info.MemTotal / (1024 * 1024 * 1024)}GB",
                    InUseRam = "N/A", // Docker API doesn't provide real-time memory usage
                    CpuUtilization = $"{info.NCPU * 100}%",
                    StorageUsed = await GetStorageUsedAlternativeAsync()
                };
            }
            catch (Exception ex)
            {
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
                return $"{totalSize / (1024 * 1024 * 1024)}GB";
            }
            catch
            {
                return "N/A";
            }
        }

        private async Task<Stream> CreateTarStreamFromDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException($"Build context directory not found: {directory}");

            // Implementation needed for tar stream creation
            throw new NotImplementedException("Tar stream creation needs to be implemented");
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
                _client?.Dispose();
            }

            _disposed = true;
        }
    }

    public class DockerOperationException : Exception
    {
        public DockerOperationException(string message, Exception innerException = null) 
            : base(message, innerException)
        {
        }
    }
}