namespace CloudHosting.Infrastructure.Model
{
    public class BuildImageRequest
    {
        public string BuildContextDir { get; set; }
        public string ImageName { get; set; }
    }

    public class RunContainerRequest
    {
        public string ImageName { get; set; }
        public string ContainerName { get; set; }
        public int PlanId { get; set; }
    }

    public class ResourceInfo
    {
        public string AvailableRam { get; set; }
        public string InUseRam { get; set; }
        public string CpuUtilization { get; set; }
        public string StorageUsed { get; set; }
    }
    
}