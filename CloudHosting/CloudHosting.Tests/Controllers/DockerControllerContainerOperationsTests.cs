using CloudHosting.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace WebApplication4.Tests.Controllers
{
    public class DockerControllerContainerOperationsTests : DockerControllerTestBase
    {
        [Fact]
        public async Task StopContainer_WhenSuccessful_ReturnsOkResult()
        {
            // Arrange
            var containerId = "test-container-id";
            MockDockerService
                .Setup(x => x.StopContainerAsync(containerId))
                .ReturnsAsync(true);

            // Act
            var result = await Controller.StopContainer(containerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.True((bool)value.Success);
        }

        [Fact]
        public async Task StopContainer_WhenFails_ReturnsBadRequest()
        {
            // Arrange
            var containerId = "test-container-id";
            MockDockerService
                .Setup(x => x.StopContainerAsync(containerId))
                .ThrowsAsync(new DockerOperationException("Failed to stop container"));

            // Act
            var result = await Controller.StopContainer(containerId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            dynamic value = badRequestResult.Value;
            Assert.Equal("Failed to stop container", (string)value.Error);
        }

        [Fact]
        public async Task GetLogs_WhenSuccessful_ReturnsOkResult()
        {
            // Arrange
            var containerId = "test-container-id";
            var expectedLogs = "Container logs content";
            MockDockerService
                .Setup(x => x.GetContainerLogsAsync(containerId))
                .ReturnsAsync(expectedLogs);

            // Act
            var result = await Controller.GetLogs(containerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal(expectedLogs, (string)value.Logs);
        }
    }
}