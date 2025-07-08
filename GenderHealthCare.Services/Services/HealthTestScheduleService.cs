using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.AvailableSlotModels;
using GenderHealthCare.ModelViews.HealthTestScheduleModels;
using GenderHealthCare.Services.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenderHealthCare.Services.Services
{
    public class HealthTestScheduleService : IHealthTestScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public HealthTestScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<HealthTestScheduleResponseModel> CreateScheduleAsync(HealthTestScheduleRequestModel model)
        {
            // Validate input
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.StartDate > model.EndDate)
                throw new ArgumentException("EndDate must be greater than StartDate");

            if (model.SlotStart >= model.SlotEnd)
                throw new ArgumentException("SlotEnd must be greater than SlotStart");

            if (model.DaysOfWeek == null || !model.DaysOfWeek.Any())
                throw new ArgumentException("At least one day of week must be specified");

            // Validate each day
            var validDays = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            var invalidDays = model.DaysOfWeek.Except(validDays).ToList();
            if (invalidDays.Any())
                throw new ArgumentException($"Invalid days specified: {string.Join(", ", invalidDays)}");

            var schedule = new HealthTestSchedule
            {
                Id = Guid.NewGuid().ToString(),
                StartDate = model.StartDate.Date, // Ensure we store only date part
                EndDate = model.EndDate.Date,     // Ensure we store only date part
                SlotStart = model.SlotStart,
                SlotEnd = model.SlotEnd,
                SlotDurationInMinutes = model.SlotDurationInMinutes,
                DaysOfWeek = string.Join(",", model.DaysOfWeek),
                HealthTestId = model.HealthTestId ?? throw new ArgumentNullException(nameof(model.HealthTestId)),
                CreatedTime = DateTimeOffset.UtcNow
            };

            await _unitOfWork.GetRepository<HealthTestSchedule>().InsertAsync(schedule);
            await _unitOfWork.SaveAsync();

            return schedule.ToHealthTestScheduleDto();
        }

        public async Task<bool> DeleteScheduleAsync(string id)
        {
            var scheduleRepo = _unitOfWork.GetRepository<HealthTestSchedule>();
            var schedule = await scheduleRepo.GetByIdAsync(id);

            if (schedule == null) return false;

            scheduleRepo.Delete(id);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<List<HealthTestScheduleResponseModel>> GetActiveSchedulesAsync(DateTime currentDate)
        {
            var schedules = await _unitOfWork.GetRepository<HealthTestSchedule>()
                .Entities
                .Where(s => s.StartDate <= currentDate && s.EndDate >= currentDate)
                .ToListAsync();

            return schedules.Select(s => s.ToHealthTestScheduleDto())
                          .OrderByDescending(s => s.CreatedTime)
                          .ToList();
        }

        public async Task<List<HealthTestScheduleResponseModel>> GetAllSchedulesAsync()
        {
            var schedules = await _unitOfWork.GetRepository<HealthTestSchedule>()
                .Entities
                .ToListAsync();

            return schedules.Select(s => s.ToHealthTestScheduleDto())
                          .OrderByDescending(s => s.CreatedTime)
                          .ToList();
        }

        public async Task<List<AvailableSlotResponse>> GetAvailableSlotsAsync(string healthTestId, DateTime date)
        {
            var dayOfWeek = date.DayOfWeek.ToString().Substring(0, 3); // "Mon", "Tue", etc.

            var schedule = await _unitOfWork.GetRepository<HealthTestSchedule>()
                .Entities
                .FirstOrDefaultAsync(s =>
                    s.HealthTestId == healthTestId &&
                    s.StartDate <= date &&
                    s.EndDate >= date &&
                    s.DaysOfWeek.Contains(dayOfWeek));

            if (schedule == null)
                return new List<AvailableSlotResponse>();

            var slots = new List<AvailableSlotResponse>();
            var currentSlotStart = schedule.SlotStart;

            while (currentSlotStart + TimeSpan.FromMinutes(schedule.SlotDurationInMinutes) <= schedule.SlotEnd)
            {
                var slotEnd = currentSlotStart.Add(TimeSpan.FromMinutes(schedule.SlotDurationInMinutes));

                slots.Add(new AvailableSlotResponse
                {
                    SlotStart = currentSlotStart,
                    SlotEnd = slotEnd,
                    IsBooked = true
                });

                currentSlotStart = slotEnd;
            }

            return slots;
        }

        public async Task<HealthTestScheduleResponseModel?> GetScheduleByIdAsync(string id)
        {
            var schedule = await _unitOfWork.GetRepository<HealthTestSchedule>()
                .Entities
                .FirstOrDefaultAsync(s => s.Id == id);

            return schedule?.ToHealthTestScheduleDto();
        }

        public async Task<List<HealthTestScheduleResponseModel>> GetSchedulesByTestIdAsync(string healthTestId)
        {
            var schedules = await _unitOfWork.GetRepository<HealthTestSchedule>()
                .Entities
                .Where(s => s.HealthTestId == healthTestId)
                .ToListAsync();

            return schedules.Select(s => s.ToHealthTestScheduleDto())
                          .OrderByDescending(s => s.CreatedTime)
                          .ToList();
        }

        public async Task<bool> UpdateScheduleAsync(string id, HealthTestScheduleRequestModel model)
        {
            var scheduleRepo = _unitOfWork.GetRepository<HealthTestSchedule>();
            var schedule = await scheduleRepo.GetByIdAsync(id);

            if (schedule == null) return false;

            schedule.StartDate = model.StartDate;
            schedule.EndDate = model.EndDate;
            schedule.SlotStart = model.SlotStart;
            schedule.SlotEnd = model.SlotEnd;
            schedule.SlotDurationInMinutes = model.SlotDurationInMinutes;
            schedule.DaysOfWeek = model.DaysOfWeek != null ? string.Join(",", model.DaysOfWeek) : "";
            schedule.HealthTestId = model.HealthTestId;

            scheduleRepo.Update(schedule);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}