using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudHosting.Domain.Entities;

namespace CloudHosting.Application.Interfaces
{
    public interface ICloudHostingRepository
    {
        Task<HostingDeployment> AddDeploymentAsync(HostingDeployment deployment);
        Task<IEnumerable<HostingDeployment>> GetUserDeploymentsAsync(Guid userId);
        Task<HostingDeployment> GetDeploymentAsync(Guid deploymentId);
        Task UpdateDeploymentStatusAsync(Guid deploymentId, string status);
    }
}