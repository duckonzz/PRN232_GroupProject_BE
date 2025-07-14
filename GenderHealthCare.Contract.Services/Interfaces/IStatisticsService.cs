using GenderHealthCare.Core.Helpers;
using GenderHealthCare.ModelViews.StatisticsModels;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<ServiceResponse<UserStatisticsResultDto>> CountAllCustomersAsync();
        Task<ServiceResponse<TestSlotStatisticsResultDto>> CountBookedTestSlotsAsync();
        Task<ServiceResponse<AvailableSlotStatisticsResultDto>> CountBookedAvailableSlotsAsync();
        Task<List<MonthlyStatisticResponse>> GetConsultationStatisticsAsync(StatisticFilterRequest request);
        Task<List<MonthlyStatisticResponse>> GetTestBookingStatisticsAsync(StatisticFilterRequest request);
        Task<RevenueStatisticsResponse> GetRevenueStatisticsAsync(DateTime? fromDate, DateTime? toDate);
    }
}
