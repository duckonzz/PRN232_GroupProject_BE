using GenderHealthCare.ModelViews.PaymentModels;
using GenderHealthCare.ModelViews.ReportModels;
using GenderHealthCare.ModelViews.VNPayModels;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IPaymentService
    {
        Task SaveVnPayResultAsync(VnPayCallbackRequest request);
        Task<List<PaymentResponseModel>> GetUserPaymentsAsync(string userId);
        Task<ExportFileResult> ExportTransactionsAsync(string userId, DateTime? from, DateTime? to, string format);
        Task<ExportFileResult> DownloadReportFileAsync(string reportId);
        Task<List<ReportResponseModel>> GetReportsAsync();

    }
}
