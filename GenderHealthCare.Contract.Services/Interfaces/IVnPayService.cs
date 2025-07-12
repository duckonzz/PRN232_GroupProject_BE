using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenderHealthCare.ModelViews.VNPayModels;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(VnPayPaymentRequest request, string ipAddress);

    }
}
