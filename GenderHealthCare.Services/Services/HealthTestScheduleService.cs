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

        public async Task<List<HealthTestScheduleResponseModel>> CreateScheduleAsync(HealthTestScheduleRequestModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.StartDate > model.EndDate)
                throw new ArgumentException("EndDate must be greater than StartDate");

            if (model.SlotStart >= model.SlotEnd)
                throw new ArgumentException("SlotEnd must be greater than SlotStart");

            if (model.SlotDurationInMinutes <= 0)
                throw new ArgumentException("Slot duration must be greater than 0");

            if (model.DaysOfWeek == null || !model.DaysOfWeek.Any())
                throw new ArgumentException("At least one day of week must be specified");

            var validDays = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            var invalidDays = model.DaysOfWeek.Except(validDays).ToList();
            if (invalidDays.Any())
                throw new ArgumentException($"Invalid days specified: {string.Join(", ", invalidDays)}");

            var createdSchedules = new List<HealthTestSchedule>();

            var totalMinutes = (int)(model.SlotEnd - model.SlotStart).TotalMinutes;

            if (totalMinutes < model.SlotDurationInMinutes)
            {
                throw new ArgumentException("The time range between SlotStart and SlotEnd must be greater than or equal to SlotDurationInMinutes.");
            }

            if (totalMinutes % model.SlotDurationInMinutes != 0)
            {
                throw new ArgumentException("The time range must be divisible by SlotDurationInMinutes.");
            }

            for (var date = model.StartDate.Date; date <= model.EndDate.Date; date = date.AddDays(1))
            {
                var dayName = date.DayOfWeek.ToString().Substring(0, 3); // "Mon", "Tue", etc.
                if (!model.DaysOfWeek.Contains(dayName)) continue;

                var currentSlotStart = model.SlotStart;
                while (currentSlotStart + TimeSpan.FromMinutes(model.SlotDurationInMinutes) <= model.SlotEnd)
                {
                    var slotEnd = currentSlotStart + TimeSpan.FromMinutes(model.SlotDurationInMinutes);

                    var schedule = new HealthTestSchedule
                    {
                        Id = Guid.NewGuid().ToString(),
                        StartDate = date,
                        EndDate = date,
                        SlotStart = currentSlotStart,
                        SlotEnd = slotEnd,
                        SlotDurationInMinutes = model.SlotDurationInMinutes,
                        DaysOfWeek = dayName,
                        HealthTestId = model.HealthTestId ?? throw new ArgumentNullException(nameof(model.HealthTestId)),
                        CreatedTime = DateTimeOffset.UtcNow
                    };

                    createdSchedules.Add(schedule);

                    currentSlotStart = slotEnd;
                }
            }

            var repo = _unitOfWork.GetRepository<HealthTestSchedule>();
            await repo.InsertRangeAsync(createdSchedules);
            await _unitOfWork.SaveAsync();

            return createdSchedules.Select(s => s.ToHealthTestScheduleDto()).ToList();
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