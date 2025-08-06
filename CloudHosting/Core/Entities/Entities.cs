namespace CloudHosting.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public List<CloudPlan> CloudPlans { get; set; } = new();
        public List<FileUpload> UploadedFiles { get; set; } = new();
    }
}

namespace CloudHosting.Core.Entities
{
    public class CloudPlan
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int MaxCpuCores { get; set; }
        public long MaxMemoryMB { get; set; }
    }
}

namespace CloudHosting.Core.Entities
{
    public class FileUpload
    {
        public int Id { get; set; }
        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public required User User { get; set; }
    }
}
