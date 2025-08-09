using Microsoft.AspNetCore.Mvc;
using CloudHosting.Infrastructure.Model;
using CloudHosting.Core.Interfaces;

namespace CloudHosting.Presentation.Controller
{
    [VerifyUser]
    [ApiController]
    [Route("api/[controller]")]
    public class DockerController : ControllerBase
    {
        private readonly IDockerService _dockerService;
        private readonly ICloudPlanService _cloudPlanService;
        private readonly IFileService _fileService; // Add IFileService

        public DockerController(IDockerService dockerService, ICloudPlanService cloudPlanService, IFileService fileService)
        {
            _dockerService = dockerService;
            _cloudPlanService = cloudPlanService;
            _fileService = fileService; // Initialize IFileService
        }

        [HttpPost("build")]
        public async Task<IActionResult> BuildImage( IFormFile file, [FromForm] string imageName, [FromForm] string userId)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                if (!file.ContentType.Equals("application/zip"))
                    return BadRequest("Only zip files are allowed");
                
                var buildPath = await _fileService.SaveAndExtractZipAsync(file, imageName, userId);

                try
                {
                    var imageId = await _dockerService.BuildImageAsync(buildPath, imageName);
                    return Ok(new { ImageId = imageId });
                }
                finally
                {
                    // Cleanup build context after build
                    _fileService.CleanupBuildContext(buildPath);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("run/{userId}")]
        public async Task<IActionResult> RunContainer(int userId, [FromBody] RunContainerRequest request)
        {
            try
            {
                var plans = await _cloudPlanService.GetActivePlansAsync(userId);
                var plan = plans.FirstOrDefault(p => p.Id == request.PlanId);
                
                if (plan == null)
                    return BadRequest(new { Error = "Invalid or expired plan" });

                var containerId = await _dockerService.RunContainerAsync(
                    request.ImageName, 
                    request.ContainerName,
                    plan);

                return Ok(new { ContainerId = containerId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("stop/{containerId}")]
        public async Task<IActionResult> StopContainer(string containerId)
        {
            try
            {
                var result = await _dockerService.StopContainerAsync(containerId);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("logs/{containerId}")]
        public async Task<IActionResult> GetLogs(string containerId)
        {
            try
            {
                var logs = await _dockerService.GetContainerLogsAsync(containerId);
                return Ok(new { Logs = logs });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("resources")]
        public async Task<IActionResult> GetResourceInfo()
        {
            try
            {
                var info = await _dockerService.GetResourceInfoAsync();
                return Ok(info);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}