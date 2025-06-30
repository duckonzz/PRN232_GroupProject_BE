using GenderHealthCare.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Repositories.Interfaces
{
    public interface IConsultantRepository
    {
        IQueryable<Consultant> Query();                 // đã Include User
        Task<Consultant?> GetByIdAsync(string id);

        Task AddAsync(Consultant consultant);
        void Update(Consultant consultant);
        void Delete(Consultant consultant);

        Task SaveChangesAsync();
    }
}
