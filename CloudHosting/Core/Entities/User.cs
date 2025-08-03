// Core/Entities/User.cs
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public List<CloudPlan> CloudPlans { get; set; }
    public List<FileUpload> UploadedFiles { get; set; }
}

// Core/Entities/CloudPlan.cs
public class CloudPlan
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int MaxCpuCores { get; set; }
    public long MaxMemoryMB { get; set; }
}

// Core/Entities/FileUpload.cs
public class FileUpload
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public User User { get; set; }
}