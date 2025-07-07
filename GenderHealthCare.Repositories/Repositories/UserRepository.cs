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
    public class UserRepository : IUserRepository
    {
        private readonly GenderHealthCareDbContext _ctx;    

        public UserRepository(GenderHealthCareDbContext ctx) => _ctx = ctx;

        public IQueryable<User> Query() => _ctx.Users.AsQueryable();

        public Task<User?> GetByIdAsync(string id) =>
            _ctx.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}
