using GenderHealthCare.ModelViews.VNPayModels;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(VnPayPaymentRequest request, string ipAddress);

    }
}
