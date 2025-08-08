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
        public string? PaymentUrl { get; set; }
    }

    public class PaymentVerification
    {
        public required string Authority { get; set; }
        public int Amount { get; set; }
        public int Status { get; set; }
        public long RefId { get; set; }
        public string? CardPan { get; set; }
        public string? CardHash { get; set; }
    }

    public class PaymentException : Exception
    {
        public PaymentException(string message) : base(message) { }
        public PaymentException(string message, Exception innerException) : base(message, innerException) { }
    }

    internal class ZarinpalResponse
    {
        public required ZarinpalData data { get; set; }
        public required List<string> errors { get; set; }
    }

    internal class ZarinpalData
    {
        public int code { get; set; }
        public string message { get; set; } = string.Empty;
        public string authority { get; set; } = string.Empty;
        public string fee_type { get; set; } = string.Empty;
        public int fee { get; set; }
    }

    internal class ZarinpalVerifyResponse
    {
        public required ZarinpalVerifyData data { get; set; }
        public required List<string> errors { get; set; }
    }

    internal class ZarinpalVerifyData
    {
        public int code { get; set; }
        public string message { get; set; } = string.Empty;
        public string card_pan { get; set; } = string.Empty;
        public string card_hash { get; set; } = string.Empty;
        public long ref_id { get; set; }
        public int fee_type { get; set; }
        public int fee { get; set; }
    }
}