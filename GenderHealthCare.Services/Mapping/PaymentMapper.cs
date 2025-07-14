using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.PaymentModels;

namespace GenderHealthCare.Services.Mapping
{
    public static class PaymmentMapper
    {
        public static PaymentResponseModel ToPaymentDto(this Payment payment)
        {
            return new PaymentResponseModel
            {
                Id = payment.Id,
                ServiceName = payment.TestSlot?.HealthTestId != null
                    ? payment.TestSlot.Schedule.HealthTest.Name
                    : "Unknown Service",
                Amount = payment.Amount,
                TxnRef = payment.TxnRef,
                OrderInfo = payment.OrderInfo,
                BankCode = payment.BankCode,
                ResponseCode = payment.ResponseCode,
                TransactionStatus = payment.TransactionStatus,
                CreatedAt = payment.CreatedAt,
                PaidAt = payment.PaidAt,
                IsSuccess = payment.IsSuccess,
                TestSlotInfo = payment.TestSlot != null
                    ? $"{payment.TestSlot.TestDate:yyyy-MM-dd} {payment.TestSlot.SlotStart:hh\\:mm}-{payment.TestSlot.SlotEnd:hh\\:mm}"
                    : null
            };
        }

        public static List<PaymentResponseModel> ToPaymentDtoList(this IEnumerable<Payment> payments) =>
            payments.Select(p => p.ToPaymentDto()).ToList();
    }
}
