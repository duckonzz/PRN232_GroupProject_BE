using GenderHealthCare.Contract.Repositories.PaggingItems;
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
        Task<string> CreateAsync(CreateTestBookingDto dto);
        Task UpdateAsync(string id, UpdateTestBookingDto dto);
        Task DeleteAsync(string id);
        Task<TestBookingDto?> GetByIdAsync(string id);
        Task<PaginatedList<TestBookingDto>> GetAllAsync(int page, int size);
        Task<PaginatedList<TestBookingDto>> SearchAsync(string? status, string? customerId, int page, int size);
    }
}
