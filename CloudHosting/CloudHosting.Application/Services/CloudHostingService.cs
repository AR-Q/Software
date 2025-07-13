using System;
using System.Threading.Tasks;
using CloudHosting.Application.Interfaces;
using CloudHosting.Domain.Entities;

namespace CloudHosting.Application.Services
{
    public class CloudHostingService
    {
        private readonly ICloudHostingRepository _repository;
        private readonly IUserService _userService; // Assume exists for user validation

        public CloudHostingService(ICloudHostingRepository repository, IUserService userService)
        {
            _repository = repository;
            _userService = userService;
        }

        public async Task<HostingDeployment> DeployAsync(Guid userId, SubscriptionPlan plan, string sourceType, string sourcePath)
        {
            // Validate user and subscription
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null || !user.HasActiveSubscription(plan))
                throw new UnauthorizedAccessException("Invalid user or subscription.");

            // Validate resource limits (simplified)
            if (!CheckResourceLimits(plan, sourcePath))
                throw new InvalidOperationException("Resource limits exceeded.");

            // Simulate deployment (e.g., Docker run)
            var deployment = new HostingDeployment
            {
                UserId = userId,
                Plan = plan,
                SourceType = sourceType,
                SourcePath = sourcePath,
                Status = "Running"
            };

            return await _repository.AddDeploymentAsync(deployment);
        }

        private bool CheckResourceLimits(SubscriptionPlan plan, string sourcePath)
        {
            // TODO: Implement actual checks (file size, etc.)
            return true;
        }
    }
}