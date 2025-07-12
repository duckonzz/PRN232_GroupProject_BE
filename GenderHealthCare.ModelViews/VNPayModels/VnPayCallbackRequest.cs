using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.VNPayModels
{
    public class VnPayCallbackRequest
    {
        public string? TxnRef { get; set; }
        public long Amount { get; set; }
        public string? OrderInfo { get; set; } 
        public string? ResponseCode { get; set; }
        public string? TransactionStatus { get; set; }
        public string? SecureHash { get; set; }
        public string? BankCode { get; set; }
    }
}
