
using GenderHealthCare.ModelViews.AvailableSlotModels;
using GenderHealthCare.ModelViews.HealthTestScheduleModels;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IHealthTestScheduleService
    {
        // Basic CRUD operations
        //Task<HealthTestScheduleResponseModel> CreateScheduleAsync(HealthTestScheduleRequestModel model);
        Task<List<HealthTestScheduleResponseModel>> CreateScheduleAsync(HealthTestScheduleRequestModel model);
        Task<HealthTestScheduleResponseModel?> GetScheduleByIdAsync(string id);
        Task<List<HealthTestScheduleResponseModel>> GetAllSchedulesAsync();
        Task<bool> UpdateScheduleAsync(string id, HealthTestScheduleRequestModel model);
        Task<bool> DeleteScheduleAsync(string id);

        // Schedule-specific methods
        Task<List<HealthTestScheduleResponseModel>> GetSchedulesByTestIdAsync(string healthTestId);
        Task<List<AvailableSlotResponse>> GetAvailableSlotsAsync(string healthTestId, DateTime date);
        Task<List<HealthTestScheduleResponseModel>> GetActiveSchedulesAsync(DateTime currentDate);
    }
}
