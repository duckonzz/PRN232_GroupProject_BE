using AutoMapper;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Constants;
using GenderHealthCare.Core.Exceptions;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.ConsultantScheduleModels;
using Microsoft.AspNetCore.Http;
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
        private readonly IAvailableSlot _availableSlotService;
        private readonly IMapper _mapper;

        public ConsultantScheduleService(
            IConsultantScheduleRepository scheduleRepo,
            IConsultantRepository consultantRepo,
            IAvailableSlot availableSlotService,
            IMapper mapper)
        {
            _scheduleRepo = scheduleRepo;
            _consultantRepo = consultantRepo;
            _availableSlotService = availableSlotService;
            _mapper = mapper;
        }

        /* ---------- CREATE ---------- */
        public async Task<ServiceResponse<string>> CreateAsync(CreateConsultantScheduleDto dto)
        {
            if (dto.StartTime >= dto.EndTime)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "End time must be after start time");

            /* 1‑3. existing duration & date window checks (unchanged) */
            if (dto.EndTime <= dto.StartTime)
                return new ServiceResponse<string> { Success = false, Message = "EndTime must be later than StartTime." };

            var today = DateTime.Today;
            var maxDate = today.AddMonths(1);
            if (dto.AvailableDate.Date < today)
                return new ServiceResponse<string> { Success = false, Message = "AvailableDate cannot be in the past." };
            if (dto.AvailableDate.Date > maxDate)
                return new ServiceResponse<string> { Success = false, Message = "AvailableDate cannot be more than one month in the future." };

            /* ---------- NEW: start time not in past if date == today ---------- */
            if (dto.AvailableDate.Date == today && dto.StartTime <= DateTime.Now.TimeOfDay)
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "StartTime must be later than the current time."
                };

            /* 4. consultant exists & 5. overlap checks (unchanged) */
            var consultant = await _consultantRepo.GetByIdAsync(dto.ConsultantId);
            if (consultant is null)
                return new ServiceResponse<string> { Success = false, Message = "Consultant not found." };

            bool overlap = await _scheduleRepo.Query().AnyAsync(s =>
                s.ConsultantId == dto.ConsultantId &&
                s.AvailableDate == dto.AvailableDate.Date &&
                dto.StartTime < s.EndTime &&
                dto.EndTime > s.StartTime);

            if (overlap)
                return new ServiceResponse<string> { Success = false, Message = "This consultant already has a schedule that overlaps the selected time. Please choose a different slot." };

            /* persist */
            dto.AvailableDate = dto.AvailableDate.Date;
            var entity = _mapper.Map<ConsultantSchedule>(dto);
            await _scheduleRepo.AddAsync(entity);
            await _scheduleRepo.SaveChangesAsync();

            // Tạo sẵn các slot trống cho lịch trình này
            await _availableSlotService.GenerateAvailableSlotsAsync(entity.Id, TimeSpan.FromMinutes(30));


            return new ServiceResponse<string> { Data = entity.Id, Success = true, Message = "Schedule created successfully" };
        }

        /* ---------- UPDATE ---------- */
        public async Task<ServiceResponse<bool>> UpdateAsync(string id, UpdateConsultantScheduleDto dto)
        {
            var entity = await _scheduleRepo.Query().FirstOrDefaultAsync(s => s.Id == id);
            if (entity is null)
                return new ServiceResponse<bool> { Success = false, Message = "Schedule not found." };

            /* consultant check + basic validation (unchanged) */
            string targetConsultantId = entity.ConsultantId;
            if (!string.IsNullOrWhiteSpace(dto.ConsultantId) && dto.ConsultantId != entity.ConsultantId)
            {
                bool exists = await _consultantRepo.Query().AnyAsync(c => c.Id == dto.ConsultantId);
                if (!exists) return new ServiceResponse<bool> { Success = false, Message = "Consultant not found." };
                targetConsultantId = dto.ConsultantId;
            }

            if (dto.EndTime <= dto.StartTime)
                return new ServiceResponse<bool> { Success = false, Message = "EndTime must be later than StartTime." };

            var today = DateTime.Today;
            var maxDate = today.AddMonths(1);
            if (dto.AvailableDate.Date < today)
                return new ServiceResponse<bool> { Success = false, Message = "AvailableDate cannot be in the past." };
            if (dto.AvailableDate.Date > maxDate)
                return new ServiceResponse<bool> { Success = false, Message = "AvailableDate cannot be more than one month in the future." };

            /* ---------- NEW: start time not in past when date == today ---------- */
            if (dto.AvailableDate.Date == today && dto.StartTime <= DateTime.Now.TimeOfDay)
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "StartTime must be later than the current time."
                };

            /* overlap check (exclude self) */
            bool overlap = await _scheduleRepo.Query().AnyAsync(s =>
                s.Id != entity.Id &&
                s.ConsultantId == targetConsultantId &&
                s.AvailableDate == dto.AvailableDate.Date &&
                dto.StartTime < s.EndTime &&
                dto.EndTime > s.StartTime);

            if (overlap)
                return new ServiceResponse<bool> { Success = false, Message = "This consultant already has a schedule that overlaps the selected time. Please choose a different slot." };

            /* apply & save */
            _mapper.Map(dto, entity);
            entity.ConsultantId = targetConsultantId;
            entity.AvailableDate = entity.AvailableDate.Date;
            _scheduleRepo.Update(entity);
            await _scheduleRepo.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true, Success = true, Message = "Schedule updated successfully" };
        }

        /* ---------- DELETE ---------- */
        public async Task<ServiceResponse<bool>> DeleteAsync(string id)
        {
            var entity = await _scheduleRepo.GetByIdAsync(id);
            if (entity is null)
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Schedule not found."
                };

            _scheduleRepo.Delete(entity);
            await _scheduleRepo.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Schedule deleted successfully"
            };
        }

        /* ---------- READ ---------- */
        public async Task<ServiceResponse<ConsultantScheduleDto>> GetByIdAsync(string id)
        {
            var entity = await _scheduleRepo.GetByIdAsync(id);
            if (entity is null)
                return new ServiceResponse<ConsultantScheduleDto>
                {
                    Success = false,
                    Message = "Schedule not found."
                };

            return new ServiceResponse<ConsultantScheduleDto>
            {
                Data = _mapper.Map<ConsultantScheduleDto>(entity),
                Success = true,
                Message = "Schedule retrieved successfully"
            };
        }

        public async Task<ServiceResponse<PaginatedList<ConsultantScheduleDto>>> GetAllAsync(int page, int size)
        {
            var q = _scheduleRepo.Query().OrderBy(s => s.AvailableDate)
                                       .ThenBy(s => s.StartTime);

            var paged = await PaginatedList<ConsultantSchedule>.CreateAsync(q, page, size);
            var dto = _mapper.Map<List<ConsultantScheduleDto>>(paged.Items);
            var result = new PaginatedList<ConsultantScheduleDto>(dto, paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<ConsultantScheduleDto>>
            {
                Data = result,
                Success = true,
                Message = "Schedule list retrieved successfully"
            };
        }

        public async Task<ServiceResponse<PaginatedList<ConsultantScheduleDto>>> SearchAsync(
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
            var result = new PaginatedList<ConsultantScheduleDto>(dto, paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<ConsultantScheduleDto>>
            {
                Data = result,
                Success = true,
                Message = "Schedule search completed successfully"
            };
        }
    }
}
