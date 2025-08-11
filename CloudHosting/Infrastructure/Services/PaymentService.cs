using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Model;
using System.Net.Http.Json;

namespace CloudHosting.Infrastructure.Services
{
    public class ZarinPalService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _merchantId;
        private const int SUCCESS_STATUS = 100;
        private const string SANDBOX_URL = "https://sandbox.zarinpal.com/";
        private const string SANDBOX_START_PAY = "https://sandbox.zarinpal.com/pg/StartPay/";

        public ZarinPalService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _merchantId = configuration["ZarinPal:MerchantId"] ?? throw new InvalidOperationException("ZarinPal:MerchantId not configured");
            _httpClient.BaseAddress = new Uri(SANDBOX_URL);
        }

        public async Task<PaymentResponse> RequestPayment(PaymentRequest request)
        {
            ValidateRequest(request);

            var zarinRequest = new
            {
                merchant_id = _merchantId,
                amount = request.Amount,
                description = request.Description,
                callback_url = request.CallbackUrl,
                metadata = new { user_id = request.UserId, plan_id = request.PlanId }
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("pg/v4/payment/request.json", zarinRequest);
                response.EnsureSuccessStatusCode();
                
                var result = await response.Content.ReadFromJsonAsync<ZarinpalResponse>();
                if (result == null)
                    throw new PaymentException("Invalid response from payment gateway");

                return new PaymentResponse 
                { 
                    Status = result.data.code,
                    Authority = result.data.authority,
                    PaymentUrl = result.data.code == SUCCESS_STATUS ? $"{SANDBOX_START_PAY}{result.data.authority}" : null
                };
            }
            catch (Exception ex)
            {
                throw new PaymentException("Failed to initiate payment", ex);
            }
        }

        public async Task<PaymentVerification> VerifyPayment(string authority, int amount)
        {
            if (string.IsNullOrEmpty(authority))
                throw new ArgumentNullException(nameof(authority));

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than 0", nameof(amount));

            var verifyRequest = new
            {
                merchant_id = _merchantId,
                amount = amount,
                authority = authority
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("pg/v4/payment/verify.json", verifyRequest);
                response.EnsureSuccessStatusCode();
                
                var result = await response.Content.ReadFromJsonAsync<ZarinpalVerifyResponse>();
                if (result == null)
                    throw new PaymentException("Invalid verification response");

                return new PaymentVerification
                {
                    Status = result.data.code,
                    Authority = authority,
                    Amount = amount,
                    RefId = result.data.ref_id,
                    CardPan = result.data.card_pan,
                    CardHash = result.data.card_hash,
                    UserId = result.data.metadata?.user_id ?? 0,
                    PlanId = result.data.metadata?.plan_id ?? 0
                };
            }
            catch (Exception ex)
            {
                throw new PaymentException("Payment verification failed", ex);
            }
        }

        private void ValidateRequest(PaymentRequest request)
        {
            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than 0", nameof(request.Amount));

            if (string.IsNullOrEmpty(request.Description))
                throw new ArgumentException("Description is required", nameof(request.Description));

            if (string.IsNullOrEmpty(request.CallbackUrl))
                throw new ArgumentException("CallbackUrl is required", nameof(request.CallbackUrl));
        }
    }
}