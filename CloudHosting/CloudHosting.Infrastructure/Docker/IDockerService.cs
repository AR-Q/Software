using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CloudHosting.Domain.Entities;

namespace CloudHosting.Infrastructure.Docker
{
    public interface IDockerService
    {
        Task<string> CreateContainerAsync(HostingDeployment deployment);
        Task<bool> StartContainerAsync(string containerId);
        Task<bool> StopContainerAsync(string containerId);
        Task<bool> DeleteContainerAsync(string containerId);
        Task<IDictionary<string, string>> GetContainerStatusAsync(string containerId);
        Task<IEnumerable<string>> GetContainerLogsAsync(string containerId);
    }
}