using Microsoft.AspNetCore.Mvc;
using WebApplication3.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class DockerController : ControllerBase
    {
        private readonly IDockerService _dockerService;
        private readonly User _user; // Placeholder for user context

        public DockerController(IDockerService dockerService)
        {
            _dockerService = dockerService;
            _user = new User // Simulated user
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                CloudPlans = new()
                {
                    new CloudPlan { Id = Guid.NewGuid(), Name = "Basic", Expiry = DateTime.UtcNow.AddDays(10), RamMb = 1024, CpuCores = 1, StorageMb = 10240 },
                    new CloudPlan { Id = Guid.NewGuid(), Name = "Expired", Expiry = DateTime.UtcNow.AddDays(-1), RamMb = 512, CpuCores = 1, StorageMb = 5120 }
                }
            };
        }

        [HttpGet("cloudplans")]
        public IActionResult GetCloudPlans()
        {
            var plans = _user.CloudPlans.Where(p => !p.IsExpired).ToList();
            return Ok(plans);
        }

        [HttpPost("build")]
        public async Task<IActionResult> BuildImage([FromBody] BuildRequest request)
        {
            var imageId = await _dockerService.BuildImageAsync(request.DockerfilePath, request.ImageName);
            return Ok(imageId);
        }

        [HttpPost("run")]
        public async Task<IActionResult> RunContainer([FromBody] RunRequest request)
        {
            var plan = _user.CloudPlans.FirstOrDefault(p => p.Id == request.CloudPlanId && !p.IsExpired);
            if (plan == null) return BadRequest("Invalid or expired cloud plan.");
            var containerId = await _dockerService.RunContainerAsync(request.ImageName, request.ContainerName, plan);
            return Ok(containerId);
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopContainer([FromBody] StopRequest request)
        {
            var result = await _dockerService.StopContainerAsync(request.ContainerId);
            return Ok(result);
        }

        [HttpGet("logs/{containerId}")]
        public async Task<IActionResult> GetLogs(string containerId)
        {
            var logs = await _dockerService.GetContainerLogsAsync(containerId);
            return Ok(logs);
        }

        [HttpGet("resourceinfo")]
        public async Task<IActionResult> GetResourceInfo()
        {
            var info = await _dockerService.GetResourceInfoAsync();
            return Ok(info);
        }

        [HttpGet("containerstatuses")]
        public async Task<IActionResult> GetContainerStatuses()
        {
            var statuses = await _dockerService.GetContainerStatusesAsync();
            return Ok(statuses);
        }
    }

    public class BuildRequest
    {
        public string DockerfilePath { get; set; }
        public string ImageName { get; set; }
    }
    public class RunRequest
    {
        public string ImageName { get; set; }
        public string ContainerName { get; set; }
        public Guid CloudPlanId { get; set; }
    }
    public class StopRequest
    {
        public string ContainerId { get; set; }
    }
}
