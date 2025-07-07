using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.AvailableSlotModels;
using GenderHealthCare.ModelViews.QueryObjects;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IAvailableSlot
    {
        Task GenerateAvailableSlotsAsync(string scheduledId, TimeSpan slotDuration);
        Task<BasePaginatedList<AvailableSlotResponse>> GetPagedAvailableSlotsAsync(AvailableSlotQueryObject queryObject);
        Task<AvailableSlotResponse> GetAvailableSlotByIdAsync(string slotId);
    }
}