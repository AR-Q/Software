using Docker.DotNet;
using Docker.DotNet.Models;
using WebApplication3.Core;
using System.IO;
using ICSharpCode.SharpZipLib.Tar;
using System.Threading;

namespace WebApplication3.Infrastructure
{
    public class DockerService : IDockerService
    {
        private readonly DockerClient _client;
        public DockerService()
        {
            _client = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
        }

        public async Task<string> BuildImageAsync(string buildContextDir, string imageName)
        {
            var parameters = new ImageBuildParameters
            {
                Dockerfile = "Dockerfile",
                Tags = new List<string> { imageName }
            };

            // Create a tar stream of the build context directory
            using (var tarStream = CreateTarStream(buildContextDir))
            {
                await _client.Images.BuildImageFromDockerfileAsync(
                    parameters,
                    tarStream,
                    null, // authConfigs
                    null, // headers
                    null, // progress
                    CancellationToken.None
                );
            }
            return imageName;
        }

        private Stream CreateTarStream(string directory)
        {
            //SharpZipLib
            var memStream = new MemoryStream();
            using (var tarOutput = new ICSharpCode.SharpZipLib.Tar.TarOutputStream(memStream, System.Text.Encoding.UTF8))
            {
                foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
                {
                    var entryName = file.Substring(directory.Length).TrimStart(Path.DirectorySeparatorChar);
                    var entry = ICSharpCode.SharpZipLib.Tar.TarEntry.CreateEntryFromFile(file);
                    entry.Name = entryName;
                    tarOutput.PutNextEntry(entry);
                    using (var fileStream = File.OpenRead(file))
                    {
                        fileStream.CopyTo(tarOutput);
                    }
                    tarOutput.CloseEntry();
                }
                tarOutput.Flush();
            }
            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;
        }

        public async Task<string> RunContainerAsync(string imageName, string containerName, CloudPlan plan)
        {
            var config = new CreateContainerParameters
            {
                Image = imageName,
                Name = containerName,
                HostConfig = new HostConfig
                {
                    Memory = plan.RamMb * 1024 * 1024,
                    NanoCPUs = plan.CpuCores * 1_000_000_000,
                    StorageOpt = new Dictionary<string, string> { ["size"] = plan.StorageMb.ToString() }
                }
            };
            var response = await _client.Containers.CreateContainerAsync(config);
            await _client.Containers.StartContainerAsync(response.ID, null);
            return response.ID;
        }

        public async Task<bool> StopContainerAsync(string containerId)
        {
            return await _client.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
        }

        public async Task<string> GetContainerLogsAsync(string containerId)
        {
            using var stream = await _client.Containers.GetContainerLogsAsync(
                containerId, 
                true, 
                new ContainerLogsParameters { ShowStdout = true, ShowStderr = true, Timestamps = false }
            );
            return "Container logs available through Docker API";
        }

        public Task<ResourceInfo> GetResourceInfoAsync()
        {
            // Placeholder: query system resources
            return Task.FromResult(new ResourceInfo
            {
                AvailableRamMb = 16000,
                UsedRamMb = 4000,
                AvailableCpuCores = 8,
                UsedCpuCores = 2,
                AvailableStorageMb = 500000,
                UsedStorageMb = 100000,
                LatencyMs = 10.5
            });
        }

        public async Task<List<Core.ContainerStatus>> GetContainerStatusesAsync()
        {
            var containers = await _client.Containers.ListContainersAsync(new ContainersListParameters { All = true });
            return containers.Select(c => new Core.ContainerStatus
            {
                ContainerId = c.ID,
                Name = c.Names?.FirstOrDefault() ?? string.Empty,
                IsRunning = c.State == "running",
                CpuUsage = 0, // Placeholder
                RamUsageMb = 0, // Placeholder
                StorageUsageMb = 0 // Placeholder
            }).ToList();
        }
    }
}
