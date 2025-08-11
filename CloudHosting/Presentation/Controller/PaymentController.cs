using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;

namespace CloudHosting.Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlansController : ControllerBase
    {
        private readonly ICloudPlanService _planService;
        private readonly IPaymentService _paymentService;

        public PlansController(ICloudPlanService planService, IPaymentService paymentService)
        {
            _planService = planService;
            _paymentService = paymentService;
        }

        [HttpGet("plans")]
        [VerifyUser]
        public async Task<IActionResult> GetPlans(int userId)
        {
            var plans = await _planService.GetActivePlansAsync(userId);
            return Ok(plans);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailablePlans()
        {
            var plans = await _planService.GetAvailablePlansAsync();
            return Ok(plans);
        }

        [HttpGet("active")]
        [VerifyUser]
        public async Task<IActionResult> GetActivePlans(int userId)
        {
            var plans = await _planService.GetActivePlansAsync(userId);
            return Ok(plans);
        }

        [HttpPost("checkout")]
        [VerifyUser]
        public async Task<IActionResult> InitiateCheckout([FromBody] PaymentRequest request)
        {
            var response = await _paymentService.RequestPayment(request);
            
            if (response.Status == 100)
            {
                return Ok(new { 
                    PaymentUrl = $"https://sandbox.zarinpal.com/pg/StartPay/{response.Authority}",
                    response.Authority
                });
            }

            return BadRequest("Payment initiation failed");
        }

        [HttpGet("verify")]
        [VerifyUser]
        public async Task<IActionResult> VerifyPayment([FromQuery] string authority, [FromQuery] int amount)
        {
            var verification = await _paymentService.VerifyPayment(authority, amount);
            
            if (verification.Status == 100)
            {
                // Add subscription to user after successful payment
                await _planService.AddUserPlanAsync(verification.UserId, verification.PlanId, verification.TransactionId);
                return Ok("Payment successful");
            }

            return BadRequest("Payment verification failed");
        }
    }
}