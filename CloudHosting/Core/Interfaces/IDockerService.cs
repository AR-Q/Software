using CloudHosting.Core.Entities;

namespace CloudHosting.Core.Interfaces
{
    public interface IDockerService
    {
        Task<string> BuildImageAsync(string buildContextDir, string imageName);
        Task<string> RunContainerAsync(string imageName, string containerName, CloudPlan plan);
        Task<bool> StopContainerAsync(string containerId);
        Task<string> GetContainerLogsAsync(string containerId);
        Task<Infrastructure.Model.ResourceInfo> GetResourceInfoAsync();
    }
}