using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.ModelViews.ConsultantScheduleModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IConsultantScheduleService
    {
        Task<string> CreateAsync(CreateConsultantScheduleDto dto);
        Task UpdateAsync(string id, UpdateConsultantScheduleDto dto);
        Task DeleteAsync(string id);

        Task<ConsultantScheduleDto?> GetByIdAsync(string id);
        Task<PaginatedList<ConsultantScheduleDto>> GetAllAsync(int page, int size);
        Task<PaginatedList<ConsultantScheduleDto>> SearchAsync(
            DateTime? availableDate,
            TimeSpan? startTime,
            TimeSpan? endTime,
            string? consultantId,
            int page, int size);
    }
}
