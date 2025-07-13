using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudHosting.Application.Interfaces;
using CloudHosting.Domain.Entities;

namespace CloudHosting.Infrastructure.Repositories
{
    public class CloudHostingRepository : ICloudHostingRepository
    {
        private readonly List<HostingDeployment> _deployments = new();

        public Task<HostingDeployment> AddDeploymentAsync(HostingDeployment deployment)
        {
            deployment.Id = Guid.NewGuid();
            deployment.CreatedAt = DateTime.UtcNow;
            _deployments.Add(deployment);
            return Task.FromResult(deployment);
        }

        public Task<IEnumerable<HostingDeployment>> GetUserDeploymentsAsync(Guid userId)
        {
            return Task.FromResult(_deployments.Where(d => d.UserId == userId));
        }

        public Task<HostingDeployment> GetDeploymentAsync(Guid deploymentId)
        {
            return Task.FromResult(_deployments.FirstOrDefault(d => d.Id == deploymentId));
        }

        public Task UpdateDeploymentStatusAsync(Guid deploymentId, string status)
        {
            var deployment = _deployments.FirstOrDefault(d => d.Id == deploymentId);
            if (deployment != null)
                deployment.Status = status;
            return Task.CompletedTask;
        }
    }
}