using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Entity;
using GenderHealthCare.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Repositories.Repositories
{
    public class TestBookingRepository : ITestBookingRepository
    {
        private readonly GenderHealthCareDbContext _context;

        public TestBookingRepository(GenderHealthCareDbContext context) => _context = context;

        public IQueryable<TestBooking> Query() =>
            _context.TestBookings.Include(t => t.Customer).Include(t => t.Slot);

        public Task<TestBooking?> GetByIdAsync(string id) =>
            Query().FirstOrDefaultAsync(t => t.Id == id);

        public Task AddAsync(TestBooking entity) =>
            _context.TestBookings.AddAsync(entity).AsTask();

        public void Delete(TestBooking entity) =>
            _context.TestBookings.Remove(entity);

        public Task SaveChangesAsync() =>
            _context.SaveChangesAsync();

        public async Task<PaginatedList<TestBooking>> SearchAsync(string? status, string? customerId, int page, int size)
        {
            var q = Query();

            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(t => t.Status == status);

            if (!string.IsNullOrWhiteSpace(customerId))
                q = q.Where(t => t.CustomerId == customerId);

            q = q.OrderByDescending(t => t.CreatedTime);

            return await PaginatedList<TestBooking>.CreateAsync(q, page, size);
        }

        public async Task<PaginatedList<TestBooking>> GetByUserAsync(
            string customerId, int page, int size)
        {
            var q = Query()
                    .Where(t => t.CustomerId == customerId)
                    .OrderByDescending(t => t.CreatedTime);

            return await PaginatedList<TestBooking>.CreateAsync(q, page, size);
        }
    }
}
