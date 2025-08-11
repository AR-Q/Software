using CloudHosting.Core.Entities;
using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Model;
using System.Text.Json;

namespace CloudHosting.Infrastructure.Services
{
    public class CloudPlanService : ICloudPlanService
    {
        private readonly ILogger<CloudPlanService> _logger;
        private const string BASE_PATH = @"C:\CloudHost\Plans";
        
        // Available plans for purchase
        private readonly List<Plan> _availablePlans = new List<Plan>
        {
            new() { Id = 1, Name = "Basic", Description = "Basic cloud hosting plan", Price = 29.99M, MaxCpuCores = 2, MaxMemoryMB = 2048, MaxStorageGB = 10, IsActive = true },
            new() { Id = 2, Name = "Pro", Description = "Professional cloud hosting plan", Price = 59.99M, MaxCpuCores = 4, MaxMemoryMB = 4096, MaxStorageGB = 20, IsActive = true }
        };

        public CloudPlanService(ILogger<CloudPlanService> logger)
        {
            _logger = logger;
        }

        public async Task<List<Plan>> GetAvailablePlansAsync()
        {
            return await Task.FromResult(_availablePlans.Where(p => p.IsActive).ToList());
        }

        public async Task<List<Model.CloudPlan>> GetActivePlansAsync(int userId)
        {
            try
            {
                var userPlanPath = Path.Combine(BASE_PATH, userId.ToString());
                if (!Directory.Exists(userPlanPath))
                {
                    _logger.LogInformation("No plans found for user: {UserId}", userId);
                    return new List<Model.CloudPlan>();
                }

                var planFiles = Directory.GetFiles(userPlanPath, "*.json");
                var activePlans = new List<Model.CloudPlan>();

                foreach (var planFile in planFiles)
                {
                    var planJson = await File.ReadAllTextAsync(planFile);
                    var plan = JsonSerializer.Deserialize<Model.CloudPlan>(planJson);
                    
                    if (plan != null && plan.ExpiryDate > DateTime.UtcNow)
                    {
                        activePlans.Add(plan);
                    }
                }

                _logger.LogInformation("Found {Count} active plans for user {UserId}", activePlans.Count, userId);
                return activePlans;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get active plans for user {UserId}", userId);
                throw;
            }
        }

        public async Task AddUserPlanAsync(int userId, int planId, string transactionId)
        {
            try
            {
                var plan = _availablePlans.FirstOrDefault(p => p.Id == planId);
                if (plan == null)
                {
                    throw new ArgumentException($"Invalid plan ID: {planId}");
                }

                var userPlanPath = Path.Combine(BASE_PATH, userId.ToString());
                Directory.CreateDirectory(userPlanPath);

                var cloudPlan = new Model.CloudPlan
                {
                    Id = planId,
                    Name = plan.Name,
                    ExpiryDate = DateTime.UtcNow.AddMonths(1),
                    MaxCpuCores = plan.MaxCpuCores,
                    MaxMemoryMB = plan.MaxMemoryMB
                };

                var planFile = Path.Combine(userPlanPath, $"{transactionId}.json");
                await File.WriteAllTextAsync(planFile, JsonSerializer.Serialize(cloudPlan));

                _logger.LogInformation("Added new plan for user {UserId}: {PlanName}", userId, plan.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add plan for user {UserId}", userId);
                throw;
            }
        }
    }
}