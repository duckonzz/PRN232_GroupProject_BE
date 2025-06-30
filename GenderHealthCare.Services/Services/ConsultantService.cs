using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.ConsultantModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Services.Services
{
    public class ConsultantService : IConsultantService
    {
        private readonly IConsultantRepository repo;
        public ConsultantService(IConsultantRepository repo) => this.repo = repo;

        /* ---------- Mapper ---------- */
        private static ConsultantDto ToDto(Consultant c) => new()
        {
            Id = c.Id,
            Degree = c.Degree,
            ExperienceYears = c.ExperienceYears,
            Bio = c.Bio,
            FullName = c.User.FullName,
            Email = c.User.Email,
            PhoneNumber = c.User.PhoneNumber,
            DateOfBirth = c.User.DateOfBirth,
            Gender = c.User.Gender,
            Role = c.User.Role,
            IsCycleTrackingOn = c.User.IsCycleTrackingOn
        };

        /* ---------- CRUD ---------- */
        public async Task<string> CreateAsync(CreateConsultantDto dto)
        {
            var entity = new Consultant
            {
                Degree = dto.Degree,
                ExperienceYears = dto.ExperienceYears,
                Bio = dto.Bio,
                User = new User
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender,
                    Role = dto.Role,
                    IsCycleTrackingOn = dto.IsCycleTrackingOn
                }
            };
            await repo.AddAsync(entity);
            await repo.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(string id, UpdateConsultantDto dto)
        {
            var c = await repo.GetByIdAsync(id)
                     ?? throw new KeyNotFoundException("Consultant not found");

            // Consultant
            c.Degree = dto.Degree;
            c.ExperienceYears = dto.ExperienceYears;
            c.Bio = dto.Bio;

            // User
            var u = c.User;
            u.FullName = dto.FullName;
            u.PhoneNumber = dto.PhoneNumber;
            u.DateOfBirth = dto.DateOfBirth;
            u.Gender = dto.Gender;
            u.Role = dto.Role;
            u.IsCycleTrackingOn = dto.IsCycleTrackingOn;

            repo.Update(c);
            await repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var c = await repo.GetByIdAsync(id)
                     ?? throw new KeyNotFoundException("Consultant not found");
            repo.Delete(c);
            await repo.SaveChangesAsync();
        }

        /* ---------- Get & Search ---------- */
        public async Task<ConsultantDto?> GetByIdAsync(string id)
            => await repo.GetByIdAsync(id) is { } c ? ToDto(c) : null;

        public async Task<PaginatedList<ConsultantDto>> GetAllAsync(int page, int size)
        {
            var q = repo.Query().OrderBy(c => c.User.FullName);
            var paged = await PaginatedList<Consultant>.CreateAsync(q, page, size);
            var dto = paged.Items.Select(ToDto).ToList();
            return new PaginatedList<ConsultantDto>(dto, paged.TotalCount, page, size);
        }

        public async Task<PaginatedList<ConsultantDto>> SearchAsync(
            string? degree, string? email, int? expYears, int page, int size)
        {
            var q = repo.Query();

            if (!string.IsNullOrWhiteSpace(degree))
                q = q.Where(c => EF.Functions.Like(c.Degree, $"%{degree}%"));

            if (!string.IsNullOrWhiteSpace(email))
                q = q.Where(c => EF.Functions.Like(c.User.Email, $"%{email}%"));

            if (expYears.HasValue)
                q = q.Where(c => c.ExperienceYears == expYears.Value);

            var paged = await PaginatedList<Consultant>.CreateAsync(q, page, size);
            var dto = paged.Items.Select(ToDto).ToList();
            return new PaginatedList<ConsultantDto>(dto, paged.TotalCount, page, size);
        }
    }
}
