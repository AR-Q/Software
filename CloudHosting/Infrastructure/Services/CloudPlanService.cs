using CloudHosting.Core.Interfaces;

public class CloudPlanService : ICloudPlanService
{
    private readonly List<CloudPlan> _cloudPlans = new List<CloudPlan>
    {
        new CloudPlan { Id = 1, Name = "Basic", ExpiryDate = DateTime.UtcNow.AddMonths(1), MaxCpuCores = 2, MaxMemoryMB = 2048 },
        new CloudPlan { Id = 2, Name = "Pro", ExpiryDate = DateTime.UtcNow.AddMonths(2), MaxCpuCores = 4, MaxMemoryMB = 4096 }
    };

    public async Task<List<CloudPlan>> GetActivePlansAsync(int userId)
    {
        return _cloudPlans
            .Where(p => p.ExpiryDate > DateTime.UtcNow)
            .ToList();
    }
}