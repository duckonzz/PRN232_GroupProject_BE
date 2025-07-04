using AutoMapper;
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
        private readonly IConsultantRepository _repo;
        private readonly IPasswordHasher _hasher;
        private readonly IMapper _mapper;

        public ConsultantService(IConsultantRepository repo,
                                 IPasswordHasher hasher,
                                 IMapper mapper)
        {
            _repo = repo;
            _hasher = hasher;
            _mapper = mapper;
        }

        /* ---------- CREATE ---------- */
        public async Task<string> CreateAsync(CreateConsultantDto dto)
        {
            var consultant = _mapper.Map<Consultant>(dto);
            consultant.User.PasswordHash = _hasher.HashPassword(dto.Password);

            await _repo.AddAsync(consultant);
            await _repo.SaveChangesAsync();
            return consultant.Id;
        }

        /* ---------- UPDATE ---------- */
        public async Task UpdateAsync(string id, UpdateConsultantDto dto)
        {
            var consultant = await _repo.Query()               // Include User
                                        .Include(c => c.User)
                                        .FirstOrDefaultAsync(c => c.Id == id)
                           ?? throw new KeyNotFoundException("Consultant not found");

            _mapper.Map(dto, consultant);          // map into Consultant
            _mapper.Map(dto, consultant.User);     // map into nested User

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
                consultant.User.PasswordHash = _hasher.HashPassword(dto.NewPassword);

            await _repo.SaveChangesAsync();
        }

        /* ---------- DELETE ---------- */
        public async Task DeleteAsync(string id)
        {
            var c = await _repo.GetByIdAsync(id)
                     ?? throw new KeyNotFoundException("Consultant not found");
            _repo.Delete(c);
            await _repo.SaveChangesAsync();
        }

        /* ---------- READ ---------- */
        public async Task<ConsultantDto?> GetByIdAsync(string id)
        {
            var entity = await _repo.Query()
                                    .Include(c => c.User)
                                    .FirstOrDefaultAsync(c => c.Id == id);
            return entity is null ? null : _mapper.Map<ConsultantDto>(entity);
        }

        public async Task<PaginatedList<ConsultantDto>> GetAllAsync(int page, int size)
        {
            var q = _repo.Query().Include(c => c.User)
                                      .OrderBy(c => c.User.FullName);
            var paged = await PaginatedList<Consultant>.CreateAsync(q, page, size);
            var dtoLst = paged.Items.Select(_mapper.Map<ConsultantDto>).ToList();
            return new PaginatedList<ConsultantDto>(dtoLst, paged.TotalCount, page, size);
        }

        public async Task<PaginatedList<ConsultantDto>> SearchAsync(
            string? degree, string? email, int? expYears, int page, int size)
        {
            var paged = await _repo.SearchAsync(degree, email, expYears, page, size);
            var dto = paged.Items.Select(_mapper.Map<ConsultantDto>).ToList();
            return new PaginatedList<ConsultantDto>(dto, paged.TotalCount, page, size);
        }
    }
}
