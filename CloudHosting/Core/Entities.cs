namespace WebApplication3.Core
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public List<UploadedFile> UploadedFiles { get; set; } = new();
        public List<CloudPlan> CloudPlans { get; set; } = new();
    }

    public class UploadedFile
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public Guid UserId { get; set; }
    }

    public class CloudPlan
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Expiry { get; set; }
        public Guid UserId { get; set; }
        public int RamMb { get; set; }
        public int CpuCores { get; set; }
        public int StorageMb { get; set; }
        public bool IsExpired => Expiry < DateTime.UtcNow;
    }
}
