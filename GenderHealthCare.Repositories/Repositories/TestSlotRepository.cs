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
    public class TestSlotRepository : ITestSlotRepository
    {
        private readonly GenderHealthCareDbContext _context;

        public TestSlotRepository(GenderHealthCareDbContext context) => _context = context;

        public IQueryable<TestSlot> Query() =>
            _context.TestSlots.Include(s => s.BookedByUser);

        public Task<TestSlot?> GetByIdAsync(string id) =>
            Query().FirstOrDefaultAsync(s => s.Id == id);

        public Task AddAsync(TestSlot entity) =>
            _context.TestSlots.AddAsync(entity).AsTask();

        public void Delete(TestSlot entity) =>
            _context.TestSlots.Remove(entity);

        public Task SaveChangesAsync() =>
            _context.SaveChangesAsync();

        public async Task<PaginatedList<TestSlot>> SearchAsync(DateTime? testDate, string? userId, int page, int size)
        {
            var q = Query();

            if (testDate.HasValue)
                q = q.Where(s => s.TestDate.Date == testDate.Value.Date);

            if (!string.IsNullOrWhiteSpace(userId))
                q = q.Where(s => s.BookedByUserId == userId);

            q = q.OrderBy(s => s.TestDate).ThenBy(s => s.SlotStart);

            return await PaginatedList<TestSlot>.CreateAsync(q, page, size);
        }

        public async Task<PaginatedList<TestSlot>> GetByUserAsync(
            string userId, int page, int size)
        {
            var q = Query()
                    .Where(s => s.BookedByUserId == userId)
                    .OrderByDescending(s => s.TestDate)
                    .ThenBy(s => s.SlotStart);

            return await PaginatedList<TestSlot>.CreateAsync(q, page, size);
        }

        public async Task<int> CountBookedTestSlotsAsync()
        {
            return await _context.TestSlots
                .Where(slot => slot.IsBooked && slot.BookedByUserId != null)
                .CountAsync();
        }
    }
}
