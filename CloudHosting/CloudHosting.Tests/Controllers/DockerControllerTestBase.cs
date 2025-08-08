using CloudHosting.Core.Interfaces;
using CloudHosting.Presentation.Controller;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace WebApplication4.Tests.Controllers
{
    public abstract class DockerControllerTestBase
    {
        protected readonly Mock<IDockerService> MockDockerService;
        protected readonly Mock<ICloudPlanService> MockCloudPlanService;
        protected readonly Mock<IFileService> MockFileService;
        protected readonly DockerController Controller;

        protected DockerControllerTestBase()
        {
            MockDockerService = new Mock<IDockerService>();
            MockCloudPlanService = new Mock<ICloudPlanService>();
            MockFileService = new Mock<IFileService>();

            Controller = new DockerController(
                MockDockerService.Object,
                MockCloudPlanService.Object,
                MockFileService.Object);

            // Setup default user context
            var claims = new List<Claim>
            {
                new Claim("userId", "1"),
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }
    }
}