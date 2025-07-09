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
    public class QAThreadRepository : IQAThreadRepository
    {
        private readonly GenderHealthCareDbContext _ctx;
        public QAThreadRepository(GenderHealthCareDbContext ctx) => _ctx = ctx;

        public IQueryable<QAThread> Query() =>
            _ctx.QAThreads
                .Include(t => t.Customer)
                .Include(t => t.Consultant)
                    .ThenInclude(c => c.User);

        public Task<QAThread?> GetByIdAsync(string id) =>
            Query().FirstOrDefaultAsync(t => t.Id == id);

        public async Task<PaginatedList<QAThread>> SearchAsync(
            string? customerId, string? consultantId, bool? answered,
            int page, int size)
        {
            var q = Query();

            if (!string.IsNullOrWhiteSpace(customerId))
                q = q.Where(t => t.CustomerId == customerId);

            if (!string.IsNullOrWhiteSpace(consultantId))
                q = q.Where(t => t.ConsultantId == consultantId);

            if (answered.HasValue)
                q = answered.Value ? q.Where(t => t.Answer != null)
                                   : q.Where(t => t.Answer == null);

            q = q.OrderByDescending(t => t.CreatedTime);

            return await PaginatedList<QAThread>.CreateAsync(q, page, size);
        }

        public Task AddAsync(QAThread t) => _ctx.QAThreads.AddAsync(t).AsTask();
        public void Update(QAThread t) => _ctx.QAThreads.Update(t);
        public void Delete(QAThread t) => _ctx.QAThreads.Remove(t);
        public Task SaveChangesAsync() => _ctx.SaveChangesAsync();

        public async Task<PaginatedList<QAThread>> GetConversationAsync(
           string customerId, string consultantId, int page, int size)
        {
            var q = Query()
                    .Where(t => t.CustomerId == customerId &&
                                t.ConsultantId == consultantId)
                    .OrderBy(t => t.CreatedTime);          // chronological

            return await PaginatedList<QAThread>.CreateAsync(q, page, size);
        }
    }
}
