using GenderHealthCare.Contract.Repositories.Interfaces;
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
    public class ConsultantRepository : IConsultantRepository
    {
        private readonly GenderHealthCareDbContext context;

        public ConsultantRepository(GenderHealthCareDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Consultant> Query() =>
            context.Consultants
                   .Include(c => c.User)    // luôn lấy kèm User
                   .AsQueryable();

        public Task<Consultant?> GetByIdAsync(string id) =>
            Query().FirstOrDefaultAsync(c => c.Id == id);

        public Task AddAsync(Consultant consultant) =>
            context.Consultants.AddAsync(consultant).AsTask();

        public void Update(Consultant consultant) =>
            context.Consultants.Update(consultant);

        public void Delete(Consultant consultant) =>
            context.Consultants.Remove(consultant);

        public Task SaveChangesAsync() => context.SaveChangesAsync();
    }
}
