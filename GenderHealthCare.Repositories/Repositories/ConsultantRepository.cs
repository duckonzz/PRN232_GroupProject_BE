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
    public class ConsultantRepository : IConsultantRepository
    {
        private readonly GenderHealthCareDbContext _context;

        public ConsultantRepository(GenderHealthCareDbContext context)
        {
            _context = context;
        }

        /* ---------------- Base query (Include User) ---------------- */
        public IQueryable<Consultant> Query() =>
            _context.Consultants
                    .Include(c => c.User);

        public Task<Consultant?> GetByIdAsync(string id) =>
            Query().FirstOrDefaultAsync(c => c.Id == id);

        /* ---------------- Search with paging ---------------- */
        public async Task<PaginatedList<Consultant>> SearchAsync(
            string? degree,
            string? email,
            int? expYears,
            int page,
            int size)
        {
            var q = Query();                      // đã Include User sẵn

            if (!string.IsNullOrWhiteSpace(degree))
                q = q.Where(c => EF.Functions.Like(c.Degree, $"%{degree}%"));

            if (!string.IsNullOrWhiteSpace(email))
                q = q.Where(c => EF.Functions.Like(c.User.Email, $"%{email}%"));

            if (expYears.HasValue)
                q = q.Where(c => c.ExperienceYears == expYears.Value);

            q = q.OrderBy(c => c.User.FullName);

            return await PaginatedList<Consultant>.CreateAsync(q, page, size);
        }

        /* ---------------- CRUD helpers ---------------- */
        public Task AddAsync(Consultant consultant) =>
            _context.Consultants.AddAsync(consultant).AsTask();

        public void Update(Consultant consultant) =>
            _context.Consultants.Update(consultant);

        public void Delete(Consultant consultant) =>
            _context.Consultants.Remove(consultant);

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
