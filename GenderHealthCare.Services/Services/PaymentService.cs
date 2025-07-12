using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.VNPayModels;
using GenderHealthCare.Repositories.Base;

namespace GenderHealthCare.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly GenderHealthCareDbContext _context;

        public PaymentService(GenderHealthCareDbContext context)
        {
            _context = context;
        }

        public async Task SaveVnPayResultAsync(VnPayCallbackRequest request)
        {
            // Tách userId và serviceId từ OrderInfo (giả sử bạn nhúng theo định dạng userId_serviceId)
            var orderParts = request.OrderInfo?.Split('_');
            var userId = orderParts?.Length > 0 ? orderParts[0] : null;
            var serviceId = orderParts?.Length > 1 ? orderParts[1] : null;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(serviceId))
                throw new Exception("OrderInfo không hợp lệ");

            var payment = new Payment
            {
                UserId = userId,
                ServiceId = serviceId,
                Amount = request.Amount,
                TxnRef = request.TxnRef ?? "",
                OrderInfo = request.OrderInfo,
                ResponseCode = request.ResponseCode,
                TransactionStatus = request.TransactionStatus,
                SecureHash = request.SecureHash,
                BankCode = request.BankCode,
                PaidAt = DateTime.UtcNow,
                IsSuccess = request.ResponseCode == "00" && request.TransactionStatus == "00"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }
    }
}
