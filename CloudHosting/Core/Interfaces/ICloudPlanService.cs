using CloudHosting.Infrastructure.Model;

namespace CloudHosting.Core.Interfaces
{
    public interface ICloudPlanService
    {
        Task<List<CloudPlan>> GetActivePlansAsync(int userId);
    }
}