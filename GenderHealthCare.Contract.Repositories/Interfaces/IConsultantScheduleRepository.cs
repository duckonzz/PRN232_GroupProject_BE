using GenderHealthCare.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Repositories.Interfaces
{
    public interface IConsultantScheduleRepository
    {
        IQueryable<ConsultantSchedule> Query();
        Task<ConsultantSchedule?> GetByIdAsync(string id);
        Task AddAsync(ConsultantSchedule schedule);
        void Update(ConsultantSchedule schedule);
        void Delete(ConsultantSchedule schedule);
        Task SaveChangesAsync();
    }
}
