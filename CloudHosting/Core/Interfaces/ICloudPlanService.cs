using CloudHosting.Core.Entities;

namespace CloudHosting.Core.Interfaces
{
    public interface ICloudPlanService
    {
        Task<List<Plan>> GetAvailablePlansAsync();
        Task<List<Infrastructure.Model.CloudPlan>> GetActivePlansAsync(int userId);
        Task AddUserPlanAsync(int userId, int planId, string transactionId);
    }
}