using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Model;
using CloudHosting.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudHosting.Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlansController : ControllerBase
    {
        private readonly ICloudPlanService _planService;
        private readonly IPaymentService _paymentService;
        private readonly IIamService _iamService;

        public PlansController(ICloudPlanService planService, IPaymentService paymentService, IIamService iamService)
        {
            _planService = planService;
            _paymentService = paymentService;
            _iamService = iamService;
        }

        private string GetTokenFromHeader()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                throw new UnauthorizedAccessException("Invalid authorization header");
                
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        [HttpGet("plans")]
        [VerifyUser]
        public async Task<IActionResult> GetPlans()
        {

            var userId = await _iamService.GetUserIdFromTokenAsync(GetTokenFromHeader());
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
        public async Task<IActionResult> GetActivePlans()
        {
            var userId = await _iamService.GetUserIdFromTokenAsync(GetTokenFromHeader());
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

        [HttpPost("verify")]
        [VerifyUser]
        public async Task<IActionResult> VerifyPayment([FromQuery] string authority, [FromQuery] int amount)
        {
            var userId = await _iamService.GetUserIdFromTokenAsync(GetTokenFromHeader());

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            var verification = await _paymentService.VerifyPayment(authority, amount);
            
            if (verification.Status == 100)
            {
                await _planService.AddUserPlanAsync(userId, verification.PlanId, verification.TransactionId);
                return Ok("Payment successful");
            }

            return BadRequest("Payment verification failed");
        }
    }
}