using GenderHealthCare.ModelViews.PaymentModels;
using GenderHealthCare.ModelViews.VNPayModels;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IPaymentService
    {
        Task SaveVnPayResultAsync(VnPayCallbackRequest request);
        Task<List<PaymentResponseModel>> GetUserPaymentsAsync(string userId);
    }
}
