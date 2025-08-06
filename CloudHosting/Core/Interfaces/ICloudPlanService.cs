using CloudHosting.Core.Entities;

namespace CloudHosting.Core.Interfaces
{
    public interface ICloudPlanService
    {
        Task<IEnumerable<CloudPlan>> GetActivePlansAsync(int userId);
        Task<CloudPlan> GetActivePlanAsync(string userId, int planId); // New method
    }
}