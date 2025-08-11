using CloudHosting.Infrastructure.Model;
namespace CloudHosting.Core.Interfaces 
{
    public interface IPaymentService
    {
        Task<PaymentResponse> RequestPayment(PaymentRequest request);
        Task<PaymentVerification> VerifyPayment(string authority, int amount);
    }
}