using CloudHosting.Infrastructure.Model;

namespace CloudHosting.Core.Interfaces
{
    public interface IDockerService
    {
        Task<string> RunContainerAsync(string imageName, string containerName, CloudPlan plan);
        Task<string> BuildImageAsync(string buildContextDir, string imageName);
        Task<bool> StopContainerAsync(string containerId);
        Task<string> GetContainerLogsAsync(string containerId);
        Task<Infrastructure.Model.ResourceInfo> GetResourceInfoAsync();
    }
}