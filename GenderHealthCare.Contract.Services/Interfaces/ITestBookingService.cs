using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.ModelViews.TestBookingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface ITestBookingService
    {
        Task<ServiceResponse<string>> CreateAsync(CreateTestBookingDto dto);
        Task<ServiceResponse<bool>> UpdateAsync(string id, UpdateTestBookingDto dto);
        Task<ServiceResponse<bool>> DeleteAsync(string id);

        Task<ServiceResponse<TestBookingDto>> GetByIdAsync(string id);
        Task<ServiceResponse<PaginatedList<TestBookingDto>>> GetAllAsync(int page, int size);
        Task<ServiceResponse<PaginatedList<TestBookingDto>>> SearchAsync(
            string? status, string? customerId, int page, int size);
    }
}
