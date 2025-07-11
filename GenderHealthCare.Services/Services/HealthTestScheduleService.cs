﻿using GenderHealthCare.Contract.Repositories.Interfaces;
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
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.StartDate > model.EndDate)
                throw new ArgumentException("EndDate must be greater than StartDate");

            if (model.SlotStart >= model.SlotEnd)
                throw new ArgumentException("SlotEnd must be greater than SlotStart");

            if (model.SlotDurationInMinutes <= 0)
                throw new ArgumentException("SlotDurationInMinutes must be greater than zero");

            if (model.DaysOfWeek == null || !model.DaysOfWeek.Any())
                throw new ArgumentException("At least one day of week must be specified");

            var validDays = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            var invalidDays = model.DaysOfWeek.Except(validDays).ToList();
            if (invalidDays.Any())
                throw new ArgumentException($"Invalid days specified: {string.Join(", ", invalidDays)}");
               
            var expectedDuration = TimeSpan.FromMinutes(model.SlotDurationInMinutes);

            var schedule = new HealthTestSchedule
            {
                StartDate = model.StartDate.Date,
                EndDate = model.EndDate.Date,
                SlotStart = model.SlotStart,
                SlotEnd = model.SlotEnd,
                SlotDurationInMinutes = model.SlotDurationInMinutes,
                DaysOfWeek = string.Join(",", model.DaysOfWeek),
                HealthTestId = model.HealthTestId ?? throw new ArgumentNullException(nameof(model.HealthTestId)),
            };

            // ✅ Check: DaysOfWeek phải match ít nhất 1 ngày trong range
            var daysToGenerate = Enumerable.Range(0, (schedule.EndDate - schedule.StartDate).Days + 1)
                .Select(offset => schedule.StartDate.AddDays(offset))
                .Where(date => model.DaysOfWeek.Contains(date.DayOfWeek.ToString().Substring(0, 3)))
                .ToList();

            if (!daysToGenerate.Any())
            {
                throw new InvalidOperationException(
                    "No slots generated because DaysOfWeek do not match any days in the selected date range."
                );
            }

            // ✅ Check: Duplicate exact same schedule
            var scheduleRepo = _unitOfWork.GetRepository<HealthTestSchedule>();

            bool exactSameExists = await scheduleRepo.Entities.AnyAsync(x =>
                x.HealthTestId == schedule.HealthTestId &&
                x.StartDate == schedule.StartDate &&
                x.EndDate == schedule.EndDate &&
                x.SlotStart == schedule.SlotStart &&
                x.SlotEnd == schedule.SlotEnd &&
                x.SlotDurationInMinutes == schedule.SlotDurationInMinutes &&
                x.DaysOfWeek == schedule.DaysOfWeek
            );

            if (exactSameExists)
            {
                throw new InvalidOperationException(
                    "A schedule with the same configuration already exists for this Health Test."
                );
            }

            // ✅ Check: Overlapping time slot with same HealthTestId
            bool isOverlapping = await scheduleRepo.AnyAsync(x =>
                x.HealthTestId == schedule.HealthTestId &&
                // Check overlapping date range
                schedule.StartDate <= x.EndDate && schedule.EndDate >= x.StartDate &&
                // Check overlapping slot time range
                schedule.SlotStart < x.SlotEnd && schedule.SlotEnd > x.SlotStart &&
                // Check overlapping DaysOfWeek
                schedule.DaysOfWeek.Split(',').Intersect(x.DaysOfWeek.Split(',')).Any()
            );

            if (isOverlapping)
            {
                throw new InvalidOperationException(
                    "This schedule overlaps with an existing schedule for this Health Test."
                );
            }


            await _unitOfWork.GetRepository<HealthTestSchedule>().InsertAsync(schedule);
            await _unitOfWork.SaveAsync();

            // ---------------- Generate TestSlots ----------------
            var slotRepo = _unitOfWork.GetRepository<TestSlot>();
            var slots = new List<TestSlot>();
            var daysToGenerate = Enumerable.Range(0, (schedule.EndDate - schedule.StartDate).Days + 1)
                .Select(offset => schedule.StartDate.AddDays(offset))
                .Where(date => model.DaysOfWeek.Contains(date.DayOfWeek.ToString().Substring(0, 3))); // "Mon", "Tue", etc

            foreach (var date in daysToGenerate)
            {
                var time = schedule.SlotStart;
                while (time + expectedDuration <= schedule.SlotEnd)
                {
                    slots.Add(new TestSlot
                    {
                        TestDate = date,
                        SlotStart = time,
                        SlotEnd = time + expectedDuration,
                        IsBooked = false,
                        HealthTestId = schedule.HealthTestId
                    });
                    time += expectedDuration;
                }
            }

            await slotRepo.InsertRangeAsync(slots);
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