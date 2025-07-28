using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApplication3.Core
{
    public interface IDockerService
    {
        Task<string> BuildImageAsync(string dockerfilePath, string imageName);
        Task<string> RunContainerAsync(string imageName, string containerName, CloudPlan plan);
        Task<bool> StopContainerAsync(string containerId);
        Task<string> GetContainerLogsAsync(string containerId);
        Task<ResourceInfo> GetResourceInfoAsync();
        Task<List<ContainerStatus>> GetContainerStatusesAsync();
    }

    public class ResourceInfo
    {
        public int AvailableRamMb { get; set; }
        public int UsedRamMb { get; set; }
        public int AvailableCpuCores { get; set; }
        public int UsedCpuCores { get; set; }
        public int AvailableStorageMb { get; set; }
        public int UsedStorageMb { get; set; }
        public double LatencyMs { get; set; }
    }

    public class ContainerStatus
    {
        public string ContainerId { get; set; }
        public string Name { get; set; }
        public bool IsRunning { get; set; }
        public double CpuUsage { get; set; }
        public double RamUsageMb { get; set; }
        public double StorageUsageMb { get; set; }
    }
}
