namespace CloudHosting.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public List<CloudPlan> CloudPlans { get; set; } = new();
        public List<FileUpload> UploadedFiles { get; set; } = new();

        public virtual ICollection<UserPlan> Plans { get; set; }
        public virtual ICollection<UserFile> Files { get; set; }

        public User()
        {
            Plans = new HashSet<UserPlan>();
            Files = new HashSet<UserFile>();
        }
    }
    public class CloudPlan
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int MaxCpuCores { get; set; }
        public long MaxMemoryMB { get; set; }
    }
    public class FileUpload
    {
        public int Id { get; set; }
        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public required User User { get; set; }
    }
    public class Plan
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int MaxCpuCores { get; set; }
        public int MaxMemoryMB { get; set; }
        public int MaxStorageGB { get; set; }
        public bool IsActive { get; set; }
    }
    public class UserPlan
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public required string Status { get; set; }
        public decimal PaidAmount { get; set; }
        public required string TransactionId { get; set; }
        
        public virtual required User User { get; set; }
        public virtual required Plan Plan { get; set; }
    }
    public class UserFile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string OriginalFileName { get; set; }
        public required string StorageFileName { get; set; } // GUID-based name
        public required string ContentType { get; set; }
        public long FileSizeBytes { get; set; }
        public DateTime UploadedAt { get; set; }
        public required string Status { get; set; } // Pending, Active, Deleted
        public required string FileType { get; set; } // BuildContext, DockerFile, etc.
        public required string StoragePath { get; set; }
        
        public virtual required User User { get; set; }
    }
}
