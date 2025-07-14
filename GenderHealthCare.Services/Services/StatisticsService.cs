using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Enums;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.StatisticsModels;
using Microsoft.EntityFrameworkCore;

namespace GenderHealthCare.Services.Services
{
    public class StatisticsService : IStatisticsService

    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepo;
        private readonly ITestSlotRepository _testSlotRepo;

        public StatisticsService(IUserRepository userRepo, ITestSlotRepository testSlotRepo, IUnitOfWork unitOfWork)
        {
            _userRepo = userRepo;
            _testSlotRepo = testSlotRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<UserStatisticsResultDto>> CountAllCustomersAsync()
        {
            var count = await _userRepo.CountAllCustomersAsync();

            return new ServiceResponse<UserStatisticsResultDto>
            {
                Data = new UserStatisticsResultDto
                {
                    CustomerCount = count
                },
                Success = true,
                Message = "Customer count retrieved successfully."
            };
        }

        public async Task<ServiceResponse<TestSlotStatisticsResultDto>> CountBookedTestSlotsAsync()
        {
            var count = await _testSlotRepo.CountBookedTestSlotsAsync();
            return new ServiceResponse<TestSlotStatisticsResultDto>
            {
                Data = new TestSlotStatisticsResultDto { BookedTestSlotCount = count },
                Success = true,
                Message = "Booked test slot count retrieved successfully."
            };
        }

        public async Task<ServiceResponse<AvailableSlotStatisticsResultDto>> CountBookedAvailableSlotsAsync()
        {
            var count = await _userRepo.CountBookedAvailableSlotsAsync();
            return new ServiceResponse<AvailableSlotStatisticsResultDto>
            {
                Data = new AvailableSlotStatisticsResultDto { BookedAvailableSlotCount = count },
                Success = true,
                Message = "Booked consultation slot count retrieved successfully."
            };
        }

        public async Task<List<MonthlyStatisticResponse>> GetConsultationStatisticsAsync(StatisticFilterRequest request)
        {
            ValidateRequest(request);

            var query = _unitOfWork.GetRepository<Consultation>()
                .Entities
                .Where(c => c.CreatedTime.Year == request.Year);

            if (request.Month.HasValue)
                query = query.Where(c => c.CreatedTime.Month == request.Month.Value);

            if (!string.IsNullOrWhiteSpace(request.ConsultantId))
                query = query.Where(c => c.ConsultantId == request.ConsultantId);

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(c => c.Status == request.Status);

            var grouped = await query
                .GroupBy(c => c.CreatedTime.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            return (request.Month.HasValue
                ? new[] { request.Month.Value }
                : Enumerable.Range(1, 12))
                .Select(m => new MonthlyStatisticResponse
                {
                    Month = m,
                    Count = grouped.FirstOrDefault(g => g.Month == m)?.Count ?? 0
                })
                .ToList();
        }

        public async Task<List<MonthlyStatisticResponse>> GetTestBookingStatisticsAsync(StatisticFilterRequest request)
        {
            ValidateRequest(request);

            var query = _unitOfWork.GetRepository<TestBooking>()
                .Entities
                .Where(tb => tb.CreatedTime.Year == request.Year);

            if (request.Month.HasValue)
                query = query.Where(tb => tb.CreatedTime.Month == request.Month.Value);

            if (!string.IsNullOrWhiteSpace(request.HealthTestId))
                query = query.Where(tb => tb.Slot.HealthTestId == request.HealthTestId);

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(tb => tb.Status == request.Status);

            var grouped = await query
                .GroupBy(tb => tb.CreatedTime.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            return (request.Month.HasValue
                ? new[] { request.Month.Value }
                : Enumerable.Range(1, 12))
                .Select(m => new MonthlyStatisticResponse
                {
                    Month = m,
                    Count = grouped.FirstOrDefault(g => g.Month == m)?.Count ?? 0
                })
                .ToList();
        }

        public async Task<RevenueStatisticsResponse> GetRevenueStatisticsAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _unitOfWork.GetRepository<Payment>().Entities
                .Include(p => p.TestSlot)
                    .ThenInclude(ts => ts.Schedule)
                    .ThenInclude(sch => sch.HealthTest)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(p => p.CreatedAt >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(p => p.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));

            var successfulPayments = query
                .Where(p => p.IsSuccess && p.TransactionStatus == "00" && p.ResponseCode == "00");

            var totalRevenue = await successfulPayments.SumAsync(p => (long?)p.Amount) ?? 0;
            var totalTransactions = await query.CountAsync();
            var successfulTransactions = await successfulPayments.CountAsync();
            var failedTransactions = totalTransactions - successfulTransactions;

            // Group theo HealthTestId & HealthTest.Name
            var revenueByHealthTest = await successfulPayments
                .GroupBy(p => new
                {
                    p.TestSlot.HealthTestId,
                    p.TestSlot.Schedule.HealthTest.Name
                })
                .Select(g => new ServiceRevenueItem
                {
                    HealthTestId = g.Key.HealthTestId,
                    HealthTestName = g.Key.Name,
                    TotalRevenue = g.Sum(p => p.Amount),
                    TransactionCount = g.Count()
                })
                .ToListAsync();

            var revenueByMonth = await successfulPayments
                .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
                .Select(g => new MonthlyRevenueItem
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(x => x.Amount)
                }).ToListAsync();


            return new RevenueStatisticsResponse
            {
                TotalRevenue = totalRevenue,
                TotalTransactions = totalTransactions,
                SuccessfulTransactions = successfulTransactions,
                FailedTransactions = failedTransactions,
                FromDate = fromDate,
                ToDate = toDate,
                RevenueByService = revenueByHealthTest,
                RevenueByMonth = revenueByMonth
            };
        }


        // Shared
        private void ValidateRequest(StatisticFilterRequest request)
        {
            if (request.Year < 2000 || request.Year > DateTime.UtcNow.Year + 10)
                throw new ArgumentOutOfRangeException(nameof(request.Year), "Year must be between 2000 and 10 years from now");
        }
    }
}
