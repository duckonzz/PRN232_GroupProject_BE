using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Core.Helpers;
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
        Task<ServiceResponse<string>> CreateAsync(CreateTestSlotDto dto);
        Task<ServiceResponse<bool>> UpdateAsync(string id, UpdateTestSlotDto dto);
        Task<ServiceResponse<bool>> DeleteAsync(string id);

        Task<ServiceResponse<TestSlotDto>> GetByIdAsync(string id);
        Task<ServiceResponse<PaginatedList<TestSlotDto>>> GetAllAsync(int page, int size);
        Task<ServiceResponse<PaginatedList<TestSlotDto>>> SearchAsync(
            DateTime? testDate, string? userId, int page, int size);
    }
}
