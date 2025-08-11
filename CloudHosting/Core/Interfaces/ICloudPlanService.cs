using CloudHosting.Core.Entities;

namespace CloudHosting.Core.Interfaces
{
    public interface ICloudPlanService
    {
        Task<List<Plan>> GetAvailablePlansAsync();
        Task<List<Infrastructure.Model.CloudPlan>> GetActivePlansAsync(string userId);
        Task AddUserPlanAsync(string userId, int planId, string transactionId);
    }
}