using AutoMapper;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Helpers;
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
        private readonly IUserRepository _userRepo;

        public ConsultantService(IConsultantRepository repo,
                                 IPasswordHasher hasher,
                                 IMapper mapper,
                                 IUserRepository userRepo)
        {
            _repo = repo;
            _hasher = hasher;
            _mapper = mapper;
            _userRepo = userRepo;
        }

        /* ---------- CREATE ---------- */
        public async Task<ServiceResponse<string>> CreateAsync(CreateConsultantDto dto)
        {
            // (Optional) duplicate checks could go here …

            var consultant = _mapper.Map<Consultant>(dto);
            consultant.User.PasswordHash = _hasher.HashPassword(dto.Password);

            await _repo.AddAsync(consultant);
            await _repo.SaveChangesAsync();

            return new ServiceResponse<string>
            {
                Data = consultant.Id,
                Success = true,
                Message = "Consultant created successfully"
            };
        }

        /* ---------- UPDATE ---------- */
        public async Task<ServiceResponse<bool>> UpdateAsync(string id, UpdateConsultantDto dto)
        {
            var consultant = await _repo.Query()
                                        .Include(c => c.User)
                                        .FirstOrDefaultAsync(c => c.Id == id);

            if (consultant is null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Consultant not found."
                };
            }

            _mapper.Map(dto, consultant);
            _mapper.Map(dto, consultant.User);

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
                consultant.User.PasswordHash = _hasher.HashPassword(dto.NewPassword);

            await _repo.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Consultant updated successfully"
            };
        }

        /* ---------- DELETE ---------- */
        public async Task<ServiceResponse<bool>> DeleteAsync(string id)
        {
            var entity = await _repo.Query()
                                    .Include(c => c.Schedules)
                                    .FirstOrDefaultAsync(c => c.Id == id);

            if (entity is null)
                return new ServiceResponse<bool> { Success = false, Message = "Consultant not found." };

            if (entity.Schedules.Any())
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Cannot delete consultant while schedules exist. Please delete or reassign schedules first."
                };

            _repo.Delete(entity);
            await _repo.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Consultant deleted successfully"
            };
        }

        /* ---------- READ ---------- */
        public async Task<ServiceResponse<ConsultantDto>> GetByIdAsync(string id)
        {
            var entity = await _repo.Query()
                                    .Include(c => c.User)
                                    .FirstOrDefaultAsync(c => c.Id == id);

            return entity is null
                ? new ServiceResponse<ConsultantDto>
                {
                    Success = false,
                    Message = "Consultant not found."
                }
                : new ServiceResponse<ConsultantDto>
                {
                    Data = _mapper.Map<ConsultantDto>(entity),
                    Success = true,
                    Message = "Consultant retrieved successfully"
                };
        }

        public async Task<ServiceResponse<PaginatedList<ConsultantDto>>> GetAllAsync(int page, int size)
        {
            var q = _repo.Query()
                              .Include(c => c.User)
                              .OrderBy(c => c.User.FullName);

            var paged = await PaginatedList<Consultant>.CreateAsync(q, page, size);
            var dtoLst = paged.Items.Select(_mapper.Map<ConsultantDto>).ToList();
            var result = new PaginatedList<ConsultantDto>(dtoLst, paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<ConsultantDto>>
            {
                Data = result,
                Success = true,
                Message = "Consultant list retrieved successfully"
            };
        }

        public async Task<ServiceResponse<PaginatedList<ConsultantDto>>> SearchAsync(
            string? degree, string? email, int? expYears,
            int page, int size)
        {
            var paged = await _repo.SearchAsync(degree, email, expYears, page, size);
            var dto = paged.Items.Select(_mapper.Map<ConsultantDto>).ToList();
            var result = new PaginatedList<ConsultantDto>(dto, paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<ConsultantDto>>
            {
                Data = result,
                Success = true,
                Message = "Consultant search completed successfully"
            };
        }

        public async Task<ServiceResponse<string>> CreateFromUserAsync(
        string userId, CreateConsultantProfileDto dto)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            bool notEligible =
                user is null ||
                !string.Equals(user.Role, "Consultant", StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(user.ConsultantStatus, "Approved", StringComparison.OrdinalIgnoreCase) ||
                user.DeletedTime.HasValue;

            if (notEligible)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "User not found or not in Approved consultant status."
                };
            }

            /* ---- 2. Ensure profile doesn't already exist ---- */
            bool exists = await _repo.Query()
                                               .AnyAsync(c => c.UserId == userId);
            if (exists)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Consultant profile already exists for this user."
                };
            }

            /* ---- 3. Create profile ---- */
            var consultant = new Consultant
            {
                UserId = userId,
                Degree = dto.Degree,
                ExperienceYears = dto.Experience,
                Bio = dto.Bio
            };

            await _repo.AddAsync(consultant);
            await _repo.SaveChangesAsync();

            return new ServiceResponse<string>
            {
                Data = consultant.Id,
                Success = true,
                Message = "Consultant profile created successfully"
            };
        }
    }
}
