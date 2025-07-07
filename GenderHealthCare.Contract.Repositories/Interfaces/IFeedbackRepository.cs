using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IFeedbackRepository
    {
        IQueryable<Feedback> Query();
        Task<Feedback?> GetByIdAsync(string id);
        Task<PaginatedList<Feedback>> SearchAsync(
            string? targetType,
            string? targetId,
            string? userId,
            int? minRating,
            int? maxRating,
            int page, int size);

        Task AddAsync(Feedback feedback);
        void Update(Feedback feedback);
        void Delete(Feedback feedback);
        Task SaveChangesAsync();
    }
}
