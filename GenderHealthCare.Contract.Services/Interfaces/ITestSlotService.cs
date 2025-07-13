using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.ModelViews.TestSlotModels;
using GenderHealthCare.Core.Helpers;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface ITestSlotService
    {
        Task<ServiceResponse<CreateTestSlotResultDto>> UpdateByIdAsync(string id, UpdateTestSlotBookingDto dto);

        Task<ServiceResponse<bool>> UpdateAsync(string id, UpdateTestSlotDto dto);
        Task<ServiceResponse<bool>> DeleteAsync(string id);

        Task<ServiceResponse<TestSlotDto>> GetByIdAsync(string id);
        Task<ServiceResponse<PaginatedList<TestSlotDto>>> GetAllAsync(int page, int size);
        Task<ServiceResponse<PaginatedList<TestSlotDto>>> SearchAsync(
            DateTime? testDate, string? userId, int page, int size);

        Task<ServiceResponse<PaginatedList<TestSlotDto>>>
            GetByUserAsync(string userId, int page, int size);
        Task<ServiceResponse<bool>> UpdateStatus(string id);

    }
}
