using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Constants;
using GenderHealthCare.Core.Exceptions;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.PaymentModels;
using GenderHealthCare.ModelViews.ReportModels;
using GenderHealthCare.ModelViews.VNPayModels;
using GenderHealthCare.Repositories.Base;
using GenderHealthCare.Services.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace GenderHealthCare.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly GenderHealthCareDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(GenderHealthCareDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<ExportFileResult> DownloadReportFileAsync(string reportId)
        {
            var report = await _unitOfWork.GetRepository<Report>().Entities
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Report not found");

            if (report.ReportType.ToLower() == "csv")
            {
                var transactions = JsonConvert.DeserializeObject<List<dynamic>>(report.Content);

                var csv = new StringBuilder();
                csv.AppendLine("TransactionId,FullName,Amount,TxnRef,CreatedAt,HealthTest");

                foreach (var t in transactions)
                {
                    csv.AppendLine($"{t.Id},{t.FullName},{t.Amount},{t.TxnRef},{t.CreatedAt},{t.HealthTestName}");
                }

                var bytes = Encoding.UTF8.GetBytes(csv.ToString());

                return new ExportFileResult
                {
                    FileName = $"Report_{reportId}.csv",
                    ContentType = "text/csv",
                    FileBytes = bytes
                };
            }

            throw new NotImplementedException("Only CSV re-download is currently supported.");
        }

        public async Task<ExportFileResult> ExportTransactionsAsync(string userId, DateTime? from, DateTime? to, string format)
        {
            var query = _unitOfWork.GetRepository<Payment>().Entities
                .Include(p => p.User)
                .Include(p => p.TestSlot)
                    .ThenInclude(ts => ts.Schedule)
                    .ThenInclude(sch => sch.HealthTest)
                .AsQueryable();

            if (from.HasValue)
                query = query.Where(p => p.CreatedAt >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(p => p.CreatedAt <= to.Value.Date.AddDays(1).AddTicks(-1));

            var data = await query
                .Where(p => p.IsSuccess && p.TransactionStatus == "00" && p.ResponseCode == "00")
                .ToListAsync();

            if (!data.Any())
                throw new ErrorException(StatusCodes.Status404NotFound, "NoData", "No successful transactions found in the selected range");

            // 1. Convert to CSV or PDF
            ExportFileResult result = format switch
            {
                "csv" => GenerateCsv(data),
                //"pdf" => GeneratePdf(data),
                _ => throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Unsupported export format")

            };

            // 2. Save tracking report
            var report = new Report
            {
                ReportType = format.ToUpper(),
                PeriodStart = from ?? data.Min(x => x.CreatedAt),
                PeriodEnd = to ?? data.Max(x => x.CreatedAt),
                Content = JsonConvert.SerializeObject(data.Select(p => new {
                    p.Id,
                    p.User.FullName,
                    p.Amount,
                    p.TxnRef,
                    p.CreatedAt,
                    HealthTestName = p.TestSlot.Schedule.HealthTest.Name
                })),
                GeneratedByUserId = userId,
                Notes = $"Exported {data.Count} transactions"
            };

            await _unitOfWork.GetRepository<Report>().InsertAsync(report);
            await _unitOfWork.SaveAsync();

            return result;
        }

        public async Task<List<ReportResponseModel>> GetReportsAsync()
        {
            var reports = await _unitOfWork.GetRepository<Report>().Entities
                .Include(r => r.GeneratedByUser)
                .OrderByDescending(r => r.CreatedTime)
                .ToListAsync();

            return  reports.Select(r => new ReportResponseModel
            {
                Id = r.Id,
                ReportType = r.ReportType,
                PeriodStart = r.PeriodStart,
                PeriodEnd = r.PeriodEnd,
                Notes = r.Notes ?? "",
                GeneratedBy = r.GeneratedByUser.FullName,
                CreatedTime = r.CreatedTime,
            }).ToList();
        }

        public async Task<List<PaymentResponseModel>> GetUserPaymentsAsync(string userId)
        {
            var paymentRepo = _unitOfWork.GetRepository<Payment>();

            var payments = await paymentRepo.Entities
                .Include(p => p.TestSlot)
                .ThenInclude(ts => ts.Schedule)
                .ThenInclude(s => s.HealthTest)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return payments.ToPaymentDtoList();
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
                PaidAt = DateTime.UtcNow.AddHours(7),
                IsSuccess = request.ResponseCode == "00" && request.TransactionStatus == "00"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        // Shared priate methods for CSV and PDF generation
        private ExportFileResult GenerateCsv(List<Payment> payments)
        {
            var sb = new StringBuilder();
            sb.AppendLine("TransactionId,FullName,Amount,TxnRef,CreatedAt,HealthTest");

            foreach (var p in payments)
            {
                var line = $"{p.Id},{p.User.FullName},{p.Amount},{p.TxnRef},{p.CreatedAt:yyyy-MM-dd},{p.TestSlot.Schedule.HealthTest.Name}";
                sb.AppendLine(line);
            }

            return new ExportFileResult
            {
                FileBytes = Encoding.UTF8.GetBytes(sb.ToString()),
                ContentType = "text/csv",
                FileName = $"TransactionReport_{DateTime.Now:yyyyMMddHHmmss}.csv"
            };
        }
    }
}
