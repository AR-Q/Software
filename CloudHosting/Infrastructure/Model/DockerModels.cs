namespace CloudHosting.Infrastructure.Model
{
    public class BuildImageRequest
    {
        public required string BuildContextDir { get; set; }
        public required string ImageName { get; set; }
    }

    public class RunContainerRequest
    {
        public required string ImageName { get; set; }
        public required string ContainerName { get; set; }
        public int PlanId { get; set; }
    }

    public class ResourceInfo
    {
        public string AvailableRam { get; set; } = "Unknown";
        public string InUseRam { get; set; } = "Unknown";
        public string CpuUtilization { get; set; } = "Unknown";
        public string StorageUsed { get; set; } = "Unknown";
    }
    
    public class CloudPlan
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int MaxCpuCores { get; set; } = 1;
        public long MaxMemoryMB { get; set; } = 512;
    }
}