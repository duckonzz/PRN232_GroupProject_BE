using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.ModelViews.ConsultantModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IConsultantService
    {
        Task<string> CreateAsync(CreateConsultantDto dto);
        Task UpdateAsync(string id, UpdateConsultantDto dto);
        Task DeleteAsync(string id);

        Task<ConsultantDto?> GetByIdAsync(string id);
        Task<PaginatedList<ConsultantDto>> GetAllAsync(int page, int size);
        Task<PaginatedList<ConsultantDto>> SearchAsync(
            string? degree, string? email, int? expYears, int page, int size);
    }
}
