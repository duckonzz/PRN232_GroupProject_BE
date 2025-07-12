using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Entity
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }

        
        public string ServiceId { get; set; }

        
        public long Amount { get; set; } 

    
        public string TxnRef { get; set; }

        public string? OrderInfo { get; set; }

        public string? BankCode { get; set; }

        public string? ResponseCode { get; set; }

        public string? TransactionStatus { get; set; }

        public string? SecureHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? PaidAt { get; set; }

        public bool IsSuccess { get; set; } = false;
    }
}
