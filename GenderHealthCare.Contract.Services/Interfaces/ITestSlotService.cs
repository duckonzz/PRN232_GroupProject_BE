using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.ModelViews.TestSlotModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface ITestSlotService
    {
        Task<string> CreateAsync(CreateTestSlotDto dto);
        Task UpdateAsync(string id, UpdateTestSlotDto dto);
        Task DeleteAsync(string id);
        Task<TestSlotDto?> GetByIdAsync(string id);
        Task<PaginatedList<TestSlotDto>> GetAllAsync(int page, int size);
        Task<PaginatedList<TestSlotDto>> SearchAsync(DateTime? testDate, string? userId, int page, int size);
    }
}
