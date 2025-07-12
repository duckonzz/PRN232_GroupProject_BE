using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Repositories.Interfaces
{
    public interface ITestSlotRepository
    {
        IQueryable<TestSlot> Query();
        Task<TestSlot?> GetByIdAsync(string id);
        Task AddAsync(TestSlot entity);
        void Delete(TestSlot entity);
        Task SaveChangesAsync();
        Task<PaginatedList<TestSlot>> SearchAsync(DateTime? testDate, string? userId, int page, int size);
        Task<PaginatedList<TestSlot>> GetByUserAsync(
          string userId, int page, int size);

        Task<int> CountBookedTestSlotsAsync();
    }
}
