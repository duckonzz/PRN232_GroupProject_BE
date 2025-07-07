using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.ModelViews.ConsultantModels;
using GenderHealthCare.Core.Helpers;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IConsultantService
    {
        /* ---------- CRUD ---------- */
        Task<ServiceResponse<string>> CreateAsync(CreateConsultantDto dto);
        Task<ServiceResponse<bool>> UpdateAsync(string id, UpdateConsultantDto dto);
        Task<ServiceResponse<bool>> DeleteAsync(string id);

        /// <summary>Create Consultant record from an approved user.</summary>
        Task<ServiceResponse<string>> CreateFromUserAsync(
            string userId, CreateConsultantProfileDto dto);

        /* ---------- READ ---------- */
        Task<ServiceResponse<ConsultantDto>> GetByIdAsync(string id);
        Task<ServiceResponse<PaginatedList<ConsultantDto>>> GetAllAsync(int page, int size);
        Task<ServiceResponse<PaginatedList<ConsultantDto>>> SearchAsync(
            string? degree, string? email, int? expYears, int page, int size);

        
    }
}
