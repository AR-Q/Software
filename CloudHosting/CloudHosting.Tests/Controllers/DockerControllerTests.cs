using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Model;
using CloudHosting.Presentation.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace WebApplication4.Tests.Controllers
{
    public class DockerControllerTests
    {
        private readonly Mock<IDockerService> _mockDockerService;
        private readonly Mock<ICloudPlanService> _mockCloudPlanService;
        private readonly Mock<IFileService> _mockFileService;
        private readonly DockerController _controller;

        public DockerControllerTests()
        {
            _mockDockerService = new Mock<IDockerService>();
            _mockCloudPlanService = new Mock<ICloudPlanService>();
            _mockFileService = new Mock<IFileService>();
            _controller = new DockerController(
                _mockDockerService.Object,
                _mockCloudPlanService.Object,
                _mockFileService.Object
            );

            // Setup default user context
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("userId", "1"),
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task BuildImage_ReturnsBadRequest_WhenNoFile()
        {
            // Arrange
            IFormFile file = null;
            string imageName = "test-image";

            // Act
            var result = await _controller.BuildImage(file, imageName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task BuildImage_ReturnsBadRequest_WhenInvalidFileType()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.ContentType).Returns("application/json");
            mockFile.Setup(f => f.Length).Returns(1024);

            // Act
            var result = await _controller.BuildImage(mockFile.Object, "test-image");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RunContainer_ReturnsOk_WithValidPlan()
        {
            // Arrange
            var userId = 1;
            var request = new RunContainerRequest 
            { 
                ImageName = "test-image",
                ContainerName = "test-container",
                PlanId = 1
            };

            var plan = new CloudPlan 
            { 
                Id = 1,
                Name = "Basic",
                MaxCpuCores = 2,
                MaxMemoryMB = 2048
            };

            _mockCloudPlanService.Setup(x => x.GetActivePlansAsync(userId))
                .ReturnsAsync(new List<CloudPlan> { plan });

            _mockDockerService.Setup(x => x.RunContainerAsync(
                request.ImageName,
                request.ContainerName,
                plan))
                .ReturnsAsync("test-container-id");

            // Act
            var result = await _controller.RunContainer(userId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic returnValue = okResult.Value;
            Assert.Equal("test-container-id", (string)returnValue.ContainerId);
        }

        [Fact]
        public async Task RunContainer_ReturnsBadRequest_WithInvalidPlan()
        {
            // Arrange
            var userId = 1;
            var request = new RunContainerRequest 
            { 
                ImageName = "test-image",
                ContainerName = "test-container",
                PlanId = 999 // Non-existent plan
            };

            _mockCloudPlanService.Setup(x => x.GetActivePlansAsync(userId))
                .ReturnsAsync(new List<CloudPlan>());

            // Act
            var result = await _controller.RunContainer(userId, request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}