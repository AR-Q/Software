using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Model;
using CloudHosting.Presentation.Controller;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace WebApplication4.Tests.Controllers
{
    public class PlansControllerTests
    {
        private readonly Mock<ICloudPlanService> _mockPlanService;
        private readonly Mock<IPaymentService> _mockPaymentService;
        private readonly PlansController _controller;

        public PlansControllerTests()
        {
            _mockPlanService = new Mock<ICloudPlanService>();
            _mockPaymentService = new Mock<IPaymentService>();
            _controller = new PlansController(_mockPlanService.Object, _mockPaymentService.Object);
        }

        [Fact]
        public async Task GetPlans_ReturnsOkResult_WithPlans()
        {
            // Arrange
            var userId = 1;
            var expectedPlans = new List<CloudPlan>
            {
                new() { Id = 1, Name = "Basic", ExpiryDate = DateTime.UtcNow.AddDays(30) },
                new() { Id = 2, Name = "Pro", ExpiryDate = DateTime.UtcNow.AddDays(60) }
            };

            _mockPlanService.Setup(x => x.GetActivePlansAsync(userId))
                .ReturnsAsync(expectedPlans);

            // Act
            var result = await _controller.GetPlans(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<CloudPlan>>(okResult.Value);
            Assert.Equal(expectedPlans.Count, returnValue.Count);
        }

        [Fact]
        public async Task InitiateCheckout_ReturnsOkResult_WhenPaymentInitiated()
        {
            // Arrange
            var request = new PaymentRequest 
            { 
                Amount = 1000,
                Description = "Test payment",
                CallbackUrl = "http://test.com/callback",
                UserId = 1,
                PlanId = 1
            };

            var response = new PaymentResponse { Status = 100, Authority = "test-authority" };

            _mockPaymentService.Setup(x => x.RequestPayment(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.InitiateCheckout(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic returnValue = okResult.Value;
            Assert.Contains("sandbox.zarinpal.com", (string)returnValue.PaymentUrl);
            Assert.Equal("test-authority", (string)returnValue.Authority);
        }

        [Fact]
        public async Task InitiateCheckout_ReturnsBadRequest_WhenPaymentFails()
        {
            // Arrange
            var request = new PaymentRequest 
            { 
                Amount = 1000,
                Description = "Test payment",
                CallbackUrl = "http://test.com/callback"
            };

            var response = new PaymentResponse { Status = 0, Authority = "" };

            _mockPaymentService.Setup(x => x.RequestPayment(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.InitiateCheckout(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}