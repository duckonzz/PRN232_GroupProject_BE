using GenderHealthCare.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IQueryable<User> Query();
        Task<User?> GetByIdAsync(string id);
        Task<int> CountAllCustomersAsync();

        Task<int> CountBookedAvailableSlotsAsync();
    }
}
