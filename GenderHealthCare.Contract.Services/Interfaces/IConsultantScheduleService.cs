using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.ModelViews.ConsultantScheduleModels;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IConsultantScheduleService
    {
        /* ---------- CRUD ---------- */
        Task<ServiceResponse<string>> CreateAsync(CreateConsultantScheduleDto dto);
        Task<ServiceResponse<bool>> UpdateAsync(string id, UpdateConsultantScheduleDto dto);
        Task<ServiceResponse<bool>> DeleteAsync(string id);

        /* ---------- READ ---------- */
        Task<ServiceResponse<ConsultantScheduleDto?>> GetByIdAsync(string id);
        Task<ServiceResponse<PaginatedList<ConsultantScheduleDto>>> GetAllAsync(int page, int size);
        Task<ServiceResponse<PaginatedList<ConsultantScheduleDto>>> SearchAsync(
            DateTime? availableDate,
            TimeSpan? startTime,
            TimeSpan? endTime,
            string? consultantId,
            int page, int size);
    }
}
