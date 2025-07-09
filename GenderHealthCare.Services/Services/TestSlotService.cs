using AutoMapper;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.TestSlotModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Services.Services
{
    public class TestSlotService : ITestSlotService
    {
        private readonly ITestSlotRepository _repo;
        private readonly IMapper _mapper;

        public TestSlotService(ITestSlotRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        /* ---------------- CREATE ---------------- */
        /* ---------------- CREATE ---------------- */
        public async Task<ServiceResponse<string>> CreateAsync(CreateTestSlotDto dto)
        {
            /* 1‑3. duration + date window checks (unchanged) */
            if (dto.SlotEnd <= dto.SlotStart)
                return new ServiceResponse<string> { Success = false, Message = "SlotEnd must be later than SlotStart." };

            var today = DateTime.Today;
            var maxDate = today.AddMonths(1);
            if (dto.TestDate.Date < today)
                return new ServiceResponse<string> { Success = false, Message = "TestDate cannot be in the past." };
            if (dto.TestDate.Date > maxDate)
                return new ServiceResponse<string> { Success = false, Message = "TestDate cannot be more than one month in the future." };

            /* --- 4. overlap check (unchanged) --- */
            bool overlap = await _repo.Query().AnyAsync(s =>
                s.HealthTestId == dto.HealthTestId &&
                s.TestDate.Date == dto.TestDate.Date &&
                dto.SlotStart < s.SlotEnd &&
                dto.SlotEnd > s.SlotStart);

            if (overlap)
                return new ServiceResponse<string> { Success = false, Message = "Another slot overlaps this time range. Choose a different slot." };

            /* --- 5. start time must be in the future if date == today --- */
            if (dto.TestDate.Date == today && dto.SlotStart <= DateTime.Now.TimeOfDay)
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "SlotStart must be later than the current time."
                };

            /* persist */
            var entity = _mapper.Map<TestSlot>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return new ServiceResponse<string> { Data = entity.Id, Success = true, Message = "Test slot created successfully" };
        }

        /* ---------------- UPDATE ---------------- */
        public async Task<ServiceResponse<bool>> UpdateAsync(string id, UpdateTestSlotDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null)
                return new ServiceResponse<bool> { Success = false, Message = "Test slot not found." };

            /* duration + date window */
            if (dto.SlotEnd <= dto.SlotStart)
                return new ServiceResponse<bool> { Success = false, Message = "SlotEnd must be later than SlotStart." };

            var today = DateTime.Today;
            var maxDate = today.AddMonths(1);
            if (dto.TestDate.Date < today)
                return new ServiceResponse<bool> { Success = false, Message = "TestDate cannot be in the past." };
            if (dto.TestDate.Date > maxDate)
                return new ServiceResponse<bool> { Success = false, Message = "TestDate cannot be more than one month in the future." };

            /* overlap check (exclude self) */
            bool overlap = await _repo.Query().AnyAsync(s =>
                s.Id != entity.Id &&
                s.HealthTestId == entity.HealthTestId &&
                s.TestDate.Date == dto.TestDate.Date &&
                dto.SlotStart < s.SlotEnd &&
                dto.SlotEnd > s.SlotStart);

            if (overlap)
                return new ServiceResponse<bool> { Success = false, Message = "Another slot overlaps this time range. Choose a different slot." };

            /* --- start time not in the past (today) --- */
            if (dto.TestDate.Date == today && dto.SlotStart <= DateTime.Now.TimeOfDay)
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "SlotStart must be later than the current time."
                };

            /* apply & save */
            _mapper.Map(dto, entity);
            await _repo.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true, Success = true, Message = "Test slot updated successfully" };
        }


        /* ---------------- DELETE ---------------- */
        public async Task<ServiceResponse<bool>> DeleteAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null)
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Test slot not found."
                };

            _repo.Delete(entity);
            await _repo.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Test slot deleted successfully"
            };
        }

        /* ---------------- READ ---------------- */
        public async Task<ServiceResponse<TestSlotDto>> GetByIdAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity is null
                ? new ServiceResponse<TestSlotDto>
                {
                    Success = false,
                    Message = "Test slot not found."
                }
                : new ServiceResponse<TestSlotDto>
                {
                    Data = _mapper.Map<TestSlotDto>(entity),
                    Success = true,
                    Message = "Retrieved successfully"
                };
        }

        public async Task<ServiceResponse<PaginatedList<TestSlotDto>>> GetAllAsync(int page, int size)
        {
            var q = _repo.Query().OrderByDescending(s => s.TestDate);
            var paged = await PaginatedList<TestSlot>.CreateAsync(q, page, size);
            var result = new PaginatedList<TestSlotDto>(
                            paged.Items.Select(_mapper.Map<TestSlotDto>).ToList(),
                            paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<TestSlotDto>>
            {
                Data = result,
                Success = true,
                Message = "Test slots retrieved successfully"
            };
        }

        public async Task<ServiceResponse<PaginatedList<TestSlotDto>>> SearchAsync(
            DateTime? testDate, string? userId, int page, int size)
        {
            var paged = await _repo.SearchAsync(testDate, userId, page, size);
            var result = new PaginatedList<TestSlotDto>(
                            paged.Items.Select(_mapper.Map<TestSlotDto>).ToList(),
                            paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<TestSlotDto>>
            {
                Data = result,
                Success = true,
                Message = "Search completed successfully"
            };
        }

        public async Task<ServiceResponse<PaginatedList<TestSlotDto>>> GetByUserAsync(string userId, int page, int size)
        {
            // tái sử dụng SearchAsync đã có: bỏ qua testDate
            var paged = await _repo.SearchAsync(null, userId, page, size);

            var result = new PaginatedList<TestSlotDto>(
                            paged.Items.Select(_mapper.Map<TestSlotDto>).ToList(),
                            paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<TestSlotDto>>
            {
                Data = result,
                Success = true,
                Message = "Lấy danh sách slot của user thành công"
            };
        }
    }
}
