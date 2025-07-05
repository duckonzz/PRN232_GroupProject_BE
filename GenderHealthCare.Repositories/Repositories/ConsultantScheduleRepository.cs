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
    public class ConsultantScheduleRepository : IConsultantScheduleRepository
    {
        private readonly GenderHealthCareDbContext _context;

        public ConsultantScheduleRepository(GenderHealthCareDbContext context) =>
            _context = context;

        public IQueryable<ConsultantSchedule> Query() =>
            _context.ConsultantSchedules
                    .Include(s => s.Consultant)
                        .ThenInclude(c => c.User)
                    .AsQueryable();

        public Task<ConsultantSchedule?> GetByIdAsync(string id) =>
            Query().FirstOrDefaultAsync(s => s.Id == id);

        public Task AddAsync(ConsultantSchedule schedule) =>
            _context.ConsultantSchedules.AddAsync(schedule).AsTask();

        public void Update(ConsultantSchedule schedule) =>
            _context.ConsultantSchedules.Update(schedule);

        public void Delete(ConsultantSchedule schedule) =>
            _context.ConsultantSchedules.Remove(schedule);

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
