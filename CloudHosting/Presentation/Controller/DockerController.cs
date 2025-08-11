using CloudHosting.Core.Entities;
using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Model;
using CloudHosting.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudHosting.Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [VerifyUser]
    public class DockerController: ControllerBase
    {
        private readonly IDockerService _dockerService;
        private readonly ICloudPlanService _cloudPlanService;
        private readonly IFileService _fileService;
        private readonly IIamService _iamService;

        public DockerController(IDockerService dockerService, ICloudPlanService cloudPlanService, IFileService fileService, IIamService iamService)
        {
            _dockerService = dockerService;
            _cloudPlanService = cloudPlanService;
            _fileService = fileService;
            _iamService = iamService;
        }

        private string GetTokenFromHeader()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                throw new UnauthorizedAccessException("Invalid authorization header");

            return authHeader.Substring("Bearer ".Length).Trim();
        }

        [HttpPost("build")]
        public async Task<IActionResult> BuildImage(IFormFile file, [FromForm] string imageName)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                if (!file.ContentType.Equals("application/zip"))
                    return BadRequest("Only zip files are allowed");

                var userId = await _iamService.GetUserIdFromTokenAsync(GetTokenFromHeader());

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
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("run")]
        public async Task<IActionResult> RunContainer([FromBody] RunContainerRequest request)
        {
            try
            {
                var userId = await _iamService.GetUserIdFromTokenAsync(GetTokenFromHeader());
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

        [HttpPost("resources")]
        public async Task<IActionResult> GetResourceInfo()
        {
            try
            {

                var userId = await _iamService.GetUserIdFromTokenAsync(GetTokenFromHeader());

                var info = await _dockerService.GetResourceInfoAsync(userId);
                return Ok(info);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("list")]
        public async Task<IActionResult> ListImages()
        {
            try
            {

                var userId = await _iamService.GetUserIdFromTokenAsync(GetTokenFromHeader());
                var images = await _fileService.GetUserDockerImagesAsync(userId);
                return Ok(new { Images = images });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}