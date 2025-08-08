namespace CloudHosting.Infrastructure.Model
{
    public class PaymentRequest
    {
        public int Amount { get; set; }
        public required string Description { get; set; }
        public required string CallbackUrl { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
    }

    public class PaymentResponse
    {
        public required string Authority { get; set; }
        public int Status { get; set; }
    }

    public class PaymentVerification
    {
        public required string Authority { get; set; }
        public int Amount { get; set; }
        public int Status { get; set; }
    }
}