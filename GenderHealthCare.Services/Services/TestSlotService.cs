using AutoMapper;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.TestSlotModel;
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

        public async Task<string> CreateAsync(CreateTestSlotDto dto)
        {
            var entity = _mapper.Map<TestSlot>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(string id, UpdateTestSlotDto dto)
        {
            var entity = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Test slot not found");
            _mapper.Map(dto, entity);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Test slot not found");
            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task<TestSlotDto?> GetByIdAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<TestSlotDto>(entity);
        }

        public async Task<PaginatedList<TestSlotDto>> GetAllAsync(int page, int size)
        {
            var q = _repo.Query().OrderByDescending(s => s.TestDate);
            var paged = await PaginatedList<TestSlot>.CreateAsync(q, page, size);
            return new PaginatedList<TestSlotDto>(
                paged.Items.Select(_mapper.Map<TestSlotDto>).ToList(),
                paged.TotalCount, page, size);
        }

        public async Task<PaginatedList<TestSlotDto>> SearchAsync(DateTime? testDate, string? userId, int page, int size)
        {
            var paged = await _repo.SearchAsync(testDate, userId, page, size);
            return new PaginatedList<TestSlotDto>(
                paged.Items.Select(_mapper.Map<TestSlotDto>).ToList(),
                paged.TotalCount, page, size);
        }
    }
}
