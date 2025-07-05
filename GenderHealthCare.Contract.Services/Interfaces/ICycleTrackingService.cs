using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.CycleTrackingModels;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface ICycleTrackingService
        
    {
        Task<CycleTrackingResponse> CreateCycleAsync(string userId, CycleTrackingRequest request);
        Task<IEnumerable<CycleTrackingResponse>> GetCyclesAsync(string userId);
        Task<CycleTrackingResponse> GetCycleByIdAsync(string cycleId, string userId);
        Task DeleteCycleAsync(string cycleId, string userId);
        Task UpdateUserCycleTrackingAsync(string userId, bool isEnabled);
    }
}
