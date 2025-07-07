using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Contract.Services.Interfaces;
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
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly GenderHealthCareDbContext _ctx;
        public FeedbackRepository(GenderHealthCareDbContext ctx) => _ctx = ctx;

        public IQueryable<Feedback> Query() =>
            _ctx.Feedbacks.Include(f => f.User).AsQueryable();

        public Task<Feedback?> GetByIdAsync(string id) =>
            Query().FirstOrDefaultAsync(f => f.Id == id);

        public async Task<PaginatedList<Feedback>> SearchAsync(
            string? targetType, string? targetId, string? userId,
            int? minRating, int? maxRating,
            int page, int size)
        {
            var q = Query();

            if (!string.IsNullOrWhiteSpace(targetType))
                q = q.Where(f => f.TargetType == targetType);

            if (!string.IsNullOrWhiteSpace(targetId))
                q = q.Where(f => f.TargetId == targetId);

            if (!string.IsNullOrWhiteSpace(userId))
                q = q.Where(f => f.UserId == userId);

            if (minRating.HasValue)
                q = q.Where(f => f.Rating >= minRating);
            if (maxRating.HasValue)
                q = q.Where(f => f.Rating <= maxRating);

            q = q.OrderByDescending(f => f.CreatedTime);
            return await PaginatedList<Feedback>.CreateAsync(q, page, size);
        }

        public Task AddAsync(Feedback fb) => _ctx.Feedbacks.AddAsync(fb).AsTask();
        public void Update(Feedback fb) => _ctx.Feedbacks.Update(fb);
        public void Delete(Feedback fb) => _ctx.Feedbacks.Remove(fb);
        public Task SaveChangesAsync() => _ctx.SaveChangesAsync();
    }
}
