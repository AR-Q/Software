using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Model;

namespace CloudHosting.Infrastructure.Services
{
    public class CloudPlanService
    {
        private readonly ILogger<CloudPlanService> _logger;
        private readonly List<CloudPlan> _cloudPlans = new List<CloudPlan>
        {
            new CloudPlan { Id = 1, Name = "Basic", ExpiryDate = DateTime.UtcNow.AddMonths(1), MaxCpuCores = 2, MaxMemoryMB = 2048 },
            new CloudPlan { Id = 2, Name = "Pro", ExpiryDate = DateTime.UtcNow.AddMonths(2), MaxCpuCores = 4, MaxMemoryMB = 4096 }
        };

        public CloudPlanService(ILogger<CloudPlanService> logger)
        {
            _logger = logger;
        }

        public Task<List<CloudPlan>> GetActivePlansAsync(int userId)
        {
            _logger.LogInformation("Getting active plans for user: {UserId}", userId);
            var activePlans = _cloudPlans
                .Where(p => p.ExpiryDate > DateTime.UtcNow)
                .ToList();
            
            _logger.LogInformation("Found {Count} active plans for user {UserId}", activePlans.Count, userId);
            return Task.FromResult(activePlans);
        }
    }
}