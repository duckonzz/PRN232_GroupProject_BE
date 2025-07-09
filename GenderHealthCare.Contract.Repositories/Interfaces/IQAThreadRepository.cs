using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Repositories.Interfaces
{
    public interface IQAThreadRepository
    {
        IQueryable<QAThread> Query();
        Task<QAThread?> GetByIdAsync(string id);

        Task<PaginatedList<QAThread>> SearchAsync(
            string? customerId,
            string? consultantId,
            bool? answered,
            int page, int size);

        Task AddAsync(QAThread thread);
        void Update(QAThread thread);
        void Delete(QAThread thread);
        Task SaveChangesAsync();

        Task<PaginatedList<QAThread>> GetConversationAsync(
           string customerId, string consultantId, int page, int size);
    }
}
