using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Model;
using System.Security.Cryptography;

namespace CloudHosting.Infrastructure.Services
{
    public class ZarinPalService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _merchantId = "N/A";

        public ZarinPalService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            if(configuration["ZarinPal:MerchantId"]!=null)
            _merchantId = configuration["ZarinPal:MerchantId"];

            // Set base address for sandbox
            _httpClient.BaseAddress = new Uri("https://sandbox.zarinpal.com/");
        }


        public async Task<PaymentResponse> RequestPayment(PaymentRequest request)
        {
            var zarinRequest = new
            {
                merchant_id = _merchantId,
                amount = request.Amount,
                description = request.Description,
                callback_url = request.CallbackUrl,
                metadata = new { user_id = request.UserId, plan_id = request.PlanId }
            };

            var response = await _httpClient.PostAsJsonAsync("pg/v4/payment/request.json", zarinRequest);
            var result = await response.Content.ReadFromJsonAsync<PaymentResponse>();
            
            return result;
        }

        public async Task<PaymentVerification> VerifyPayment(string authority, int amount)
        {
            var verifyRequest = new
            {
                merchant_id = _merchantId,
                amount = amount,
                authority = authority
            };

            var response = await _httpClient.PostAsJsonAsync("pg/v4/payment/verify.json", verifyRequest);
            var result = await response.Content.ReadFromJsonAsync<PaymentVerification>();
            
            return result;
        }
    }
}