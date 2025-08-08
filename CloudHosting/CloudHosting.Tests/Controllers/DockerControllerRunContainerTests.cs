using CloudHosting.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace WebApplication4.Tests.Controllers
{
    public class DockerControllerRunContainerTests : DockerControllerTestBase
    {
        [Fact]
        public async Task RunContainer_WithValidPlan_ReturnsOkResult()
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
                MaxMemoryMB = 2048,
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            };

            MockCloudPlanService
                .Setup(x => x.GetActivePlansAsync(userId))
                .ReturnsAsync(new List<CloudPlan> { plan });

            MockDockerService
                .Setup(x => x.RunContainerAsync(
                    request.ImageName,
                    request.ContainerName,
                    It.IsAny<CloudPlan>()))
                .ReturnsAsync("test-container-id");

            // Act
            var result = await Controller.RunContainer(userId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("test-container-id", (string)value.ContainerId);
        }

        [Fact]
        public async Task RunContainer_WithInvalidPlan_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var request = new RunContainerRequest
            {
                ImageName = "test-image",
                ContainerName = "test-container",
                PlanId = 999
            };

            MockCloudPlanService
                .Setup(x => x.GetActivePlansAsync(userId))
                .ReturnsAsync(new List<CloudPlan>());

            // Act
            var result = await Controller.RunContainer(userId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            dynamic value = badRequestResult.Value;
            Assert.Equal("Invalid or expired plan", (string)value.Error);
        }
    }
}