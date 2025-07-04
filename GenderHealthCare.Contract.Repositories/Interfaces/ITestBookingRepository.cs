using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Repositories.Interfaces
{
    public interface ITestBookingRepository
    {
        IQueryable<TestBooking> Query();
        Task<TestBooking?> GetByIdAsync(string id);
        Task AddAsync(TestBooking entity);
        void Delete(TestBooking entity);
        Task SaveChangesAsync();
        Task<PaginatedList<TestBooking>> SearchAsync(string? status, string? customerId, int page, int size);
    }
}
