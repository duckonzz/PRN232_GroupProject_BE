namespace GenderHealthCare.Entity
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public string ServiceId { get; set; } // chính là TestSlotId
        public long Amount { get; set; } 
        public string TxnRef { get; set; }
        public string? OrderInfo { get; set; }
        public string? BankCode { get; set; }
        public string? ResponseCode { get; set; }
        public string? TransactionStatus { get; set; }
        public string? SecureHash { get; set; }

        public User User { get; set; }
        public TestSlot TestSlot { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime? PaidAt { get; set; }
        public bool IsSuccess { get; set; } = false;
    }
}
