using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.VNPayModels
{
    public class VnPayPaymentRequest

    {
        public int Amount { get; set; }
        public string UserId { get; set; }
        public string ServiceId { get; set; }
    }
}
