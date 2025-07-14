namespace GenderHealthCare.ModelViews.PaymentModels
{
    public class PaymentResponseModel
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; }
        public long Amount { get; set; }
        public string TxnRef { get; set; }
        public string? OrderInfo { get; set; }
        public string? BankCode { get; set; }
        public string? ResponseCode { get; set; }
        public string? TransactionStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public bool IsSuccess { get; set; }
        public string? TestSlotInfo { get; set; } // Eg: "2025-07-21 08:00-09:00"
    }

}
