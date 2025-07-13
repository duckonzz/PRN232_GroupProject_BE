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


        // Shared
        private void ValidateRequest(StatisticFilterRequest request)
        {
            if (request.Year < 2000 || request.Year > DateTime.UtcNow.Year + 10)
                throw new ArgumentOutOfRangeException(nameof(request.Year), "Year must be between 2000 and 10 years from now");
        }
    }
}
