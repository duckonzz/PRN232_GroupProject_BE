using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Constants;
using GenderHealthCare.Core.Exceptions;
using GenderHealthCare.Core.Models;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.AvailableSlotModels;
using GenderHealthCare.ModelViews.QueryObjects;
using GenderHealthCare.Services.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GenderHealthCare.Services.Services
{
    public class AvailableSlotService : IAvailableSlot
    {
        private readonly IUnitOfWork _unitOfWork;
        public AvailableSlotService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task GenerateAvailableSlotsAsync(string scheduledId, TimeSpan slotDuration)
        {
            var scheduleRepo = _unitOfWork.GetRepository<ConsultantSchedule>();
            var slotRepo = _unitOfWork.GetRepository<AvailableSlot>();

            var schedule = await scheduleRepo.Entities
                .Include(s => s.Slots)
                .FirstOrDefaultAsync(s => s.Id == scheduledId && !s.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Schedule not found or has been deleted");

            if (schedule.Slots != null && schedule.Slots.Count != 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Available slots already exist for this schedule");
            }

            // Generate available slots based on the schedule's start and end times
            var startTime = schedule.StartTime;
            var endTime = schedule.EndTime;

            var availableSlots = new List<AvailableSlot>();

            while (startTime + slotDuration <= endTime)
            {
                availableSlots.Add(new AvailableSlot
                {
                    ScheduleId = scheduledId,
                    SlotStart = startTime,
                    SlotEnd = startTime + slotDuration
                });

                startTime += slotDuration;
            }

            await slotRepo.InsertRangeAsync(availableSlots);
            await _unitOfWork.SaveAsync();
        }

        public async Task<AvailableSlotResponse> GetAvailableSlotByIdAsync(string slotId)
        {
            var slotRepo = _unitOfWork.GetRepository<AvailableSlot>();
            var slot = await slotRepo.Entities
                .AsNoTracking()
                .Include(s => s.Schedule)
                .FirstOrDefaultAsync(s => s.Id == slotId && !s.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Available slot not found or has been deleted");

            return slot.ToAvailableSlotDto();
        }

        public async Task<BasePaginatedList<AvailableSlotResponse>> GetPagedAvailableSlotsAsync(AvailableSlotQueryObject query)
        {
            var slotRepo = _unitOfWork.GetRepository<AvailableSlot>();
            var slotsQuery = slotRepo.Entities
                .AsNoTracking()
                .Include(s => s.Schedule)
                .Where(s => !s.DeletedTime.HasValue);
                
            if(!string.IsNullOrWhiteSpace(query.ConsultantId))
                slotsQuery = slotsQuery.Where(s => s.Schedule.ConsultantId == query.ConsultantId);


            if(!string.IsNullOrWhiteSpace(query.ScheduleId))
                slotsQuery = slotsQuery.Where(s => s.ScheduleId == query.ScheduleId);

            if(query.Date.HasValue)
            {
                slotsQuery = slotsQuery.Where(s => s.Schedule.AvailableDate == query.Date.Value.Date);
            }

            if(query.FromTime.HasValue && query.ToTime.HasValue)
            {
                slotsQuery = slotsQuery.Where(s => s.SlotStart >= query.FromTime.Value && s.SlotEnd <= query.ToTime.Value);
            }

            if (query.IsBooked.HasValue)
            {
                slotsQuery = slotsQuery.Where(s => s.IsBooked == query.IsBooked.Value);
            }

            // Sorting
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                slotsQuery = query.SortBy switch
                {
                    "SlotStart" => query.IsDescending ? slotsQuery.OrderByDescending(s => s.SlotStart) : slotsQuery.OrderBy(s => s.SlotStart),
                    "CreatedTime" => query.IsDescending ? slotsQuery.OrderByDescending(s => s.CreatedTime) : slotsQuery.OrderBy(s => s.CreatedTime),
                    _ => slotsQuery.OrderBy(s => s.SlotStart)
                };
            }
            else
            {
                slotsQuery = slotsQuery.OrderBy(s => s.SlotStart);
            }

            // Paging
            var totalCount = await slotsQuery.CountAsync();
            var pagedSlots = await slotsQuery
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var dto = pagedSlots.ToAvailableSlotDtoList();

            return new BasePaginatedList<AvailableSlotResponse>(dto, totalCount, query.PageIndex, query.PageSize);
        }
    }
}
