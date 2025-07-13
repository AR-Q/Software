using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudHosting.Domain.Entities;

namespace CloudHosting.Infrastructure.Docker
{
    public class DockerService : IDockerService
    {
        private readonly IDockerClient _dockerClient;

        public DockerService()
        {
            _dockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"))
                .CreateClient();
        }

        public async Task<string> CreateContainerAsync(HostingDeployment deployment)
        {
            var response = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = deployment.SourceType == "Dockerfile" ? deployment.SourcePath : "default-runtime",
                Name = $"hosting-{deployment.Id}",
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    { "80/tcp", default }
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { "80/tcp", new List<PortBinding> { new PortBinding { HostPort = "0" } } }
                    },
                    Memory = GetMemoryLimit(deployment.Plan)
                }
            });

            return response.ID;
        }

        public async Task<bool> StartContainerAsync(string containerId)
        {
            return await _dockerClient.Containers.StartContainerAsync(containerId, null);
        }

        public async Task<bool> StopContainerAsync(string containerId)
        {
            return await _dockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
        }

        public async Task<bool> DeleteContainerAsync(string containerId)
        {
            await _dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters());
            return true;
        }

        public async Task<IDictionary<string, string>> GetContainerStatusAsync(string containerId)
        {
            var response = await _dockerClient.Containers.InspectContainerAsync(containerId);
            return new Dictionary<string, string>
            {
                { "Status", response.State.Status },
                { "Health", response.State.Health?.Status ?? "N/A" },
                { "StartedAt", response.State.StartedAt.ToString() }
            };
        }

        public async Task<IEnumerable<string>> GetContainerLogsAsync(string containerId)
        {
            var logs = await _dockerClient.Containers.GetContainerLogsAsync(containerId, 
                new ContainerLogsParameters { ShowStdout = true, ShowStderr = true });
            
            // Process logs stream and return as string collection
            return new List<string>(); // Implementation needed
        }

        private long GetMemoryLimit(SubscriptionPlan plan) => plan switch
        {
            SubscriptionPlan.Free => 512 * 1024 * 1024,        // 512MB
            SubscriptionPlan.Basic => 1024 * 1024 * 1024,      // 1GB
            SubscriptionPlan.Professional => 2048 * 1024 * 1024,// 2GB
            SubscriptionPlan.Enterprise => 4096 * 1024 * 1024,  // 4GB
            _ => 512 * 1024 * 1024
        };
    }
}