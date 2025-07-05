using AutoMapper;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.ConsultantScheduleModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Services.Services
{
    public class ConsultantScheduleService : IConsultantScheduleService
    {
        private readonly IConsultantScheduleRepository _scheduleRepo;
        private readonly IConsultantRepository _consultantRepo;
        private readonly IMapper _mapper;

        public ConsultantScheduleService(
            IConsultantScheduleRepository scheduleRepo,
            IConsultantRepository consultantRepo,
            IMapper mapper)
        {
            _scheduleRepo = scheduleRepo;
            _consultantRepo = consultantRepo;
            _mapper = mapper;
        }

        /* ---------- CRUD ---------- */
        public async Task<string> CreateAsync(CreateConsultantScheduleDto dto)
        {
            var consultant = await _consultantRepo.GetByIdAsync(dto.ConsultantId);
            if (consultant == null)
                throw new KeyNotFoundException("Consultant not found");
            dto.AvailableDate = dto.AvailableDate.Date;

            var entity = _mapper.Map<ConsultantSchedule>(dto);
            await _scheduleRepo.AddAsync(entity);
            await _scheduleRepo.SaveChangesAsync();
            return entity.Id;
        }


        public async Task UpdateAsync(string id, UpdateConsultantScheduleDto dto)
        {
            var entity = await _scheduleRepo.Query()
                                            .FirstOrDefaultAsync(s => s.Id == id)
                         ?? throw new KeyNotFoundException("Schedule not found");

            // Nếu client muốn đổi Consultant, phải kiểm tra tồn tại
            if (!string.IsNullOrWhiteSpace(dto.ConsultantId) &&
                dto.ConsultantId != entity.ConsultantId)
            {
                bool consultantExists = await _consultantRepo.Query()
                                                              .AnyAsync(c => c.Id == dto.ConsultantId);
                if (!consultantExists)
                    throw new KeyNotFoundException("Consultant not found");
            }

            // Cập nhật   
            _mapper.Map(dto, entity);
            entity.AvailableDate = entity.AvailableDate.Date; // chuẩn hoá lại
            _scheduleRepo.Update(entity);
            await _scheduleRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _scheduleRepo.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException("Schedule not found");

            _scheduleRepo.Delete(entity);
            await _scheduleRepo.SaveChangesAsync();
        }

        /* ---------- Read / Search ---------- */
        public async Task<ConsultantScheduleDto?> GetByIdAsync(string id)
        {
            var entity = await _scheduleRepo.GetByIdAsync(id);
            return entity is null ? null : _mapper.Map<ConsultantScheduleDto>(entity);
        }

        public async Task<PaginatedList<ConsultantScheduleDto>> GetAllAsync(int page, int size)
        {
            var q = _scheduleRepo.Query().OrderBy(s => s.AvailableDate).ThenBy(s => s.StartTime);
            var paged = await PaginatedList<ConsultantSchedule>.CreateAsync(q, page, size);
            var dto = _mapper.Map<List<ConsultantScheduleDto>>(paged.Items);
            return new PaginatedList<ConsultantScheduleDto>(dto, paged.TotalCount, page, size);
        }

        public async Task<PaginatedList<ConsultantScheduleDto>> SearchAsync(
            DateTime? availableDate,
            TimeSpan? startTime,
            TimeSpan? endTime,
            string? consultantId,
            int page, int size)
        {
            var q = _scheduleRepo.Query();

            if (availableDate.HasValue)
                q = q.Where(s => s.AvailableDate.Date == availableDate.Value.Date);

            if (startTime.HasValue)
                q = q.Where(s => s.StartTime == startTime.Value);

            if (endTime.HasValue)
                q = q.Where(s => s.EndTime == endTime.Value);

            if (!string.IsNullOrWhiteSpace(consultantId))
                q = q.Where(s => s.ConsultantId == consultantId);

            q = q.OrderBy(s => s.AvailableDate).ThenBy(s => s.StartTime);

            var paged = await PaginatedList<ConsultantSchedule>.CreateAsync(q, page, size);
            var dto = _mapper.Map<List<ConsultantScheduleDto>>(paged.Items);
            return new PaginatedList<ConsultantScheduleDto>(dto, paged.TotalCount, page, size);
        }
    }
}
