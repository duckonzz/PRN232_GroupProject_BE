using AutoMapper;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Enums;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.TestBookingModels;
using GenderHealthCare.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Services.Services
{
    public class TestBookingService : ITestBookingService
    {
        private readonly ITestBookingRepository _repo;
        private readonly GenderHealthCareDbContext _ctx;
        private readonly IMapper _mapper;
        public TestBookingService(ITestBookingRepository repo,
                                  GenderHealthCareDbContext ctx,
                                  IMapper mapper)
        {
            _repo = repo; _ctx = ctx; _mapper = mapper;
        }

        /* ---------------- CREATE ---------------- */
        public async Task<ServiceResponse<string>> CreateAsync(CreateTestBookingDto dto)
        {
            var slot = await _ctx.TestSlots.FirstOrDefaultAsync(s => s.Id == dto.SlotId);
            if (slot is null)
                return new ServiceResponse<string> { Success = false, Message = "TestSlot not found." };
            if (slot.IsBooked)
                return new ServiceResponse<string> { Success = false, Message = "This slot is already booked." };

            var booking = _mapper.Map<TestBooking>(dto);
            booking.Status = TestBookingStatus.Pending.ToString();

            await _repo.AddAsync(booking);

            slot.IsBooked = true;
            slot.BookedByUserId = dto.CustomerId;
            slot.BookedAt = DateTimeOffset.UtcNow;

            await _repo.SaveChangesAsync();

            return new ServiceResponse<string>
            { Data = booking.Id, Success = true, Message = "Created successfully" };
        }

        /* ---------------- UPDATE ---------------- */
        public async Task<ServiceResponse<bool>> UpdateAsync(string id, UpdateTestBookingDto dto)
        {
            var booking = await _ctx.TestBookings.Include(b => b.Slot)
                                                 .FirstOrDefaultAsync(b => b.Id == id);
            if (booking is null)
                return new ServiceResponse<bool> { Success = false, Message = "TestBooking not found." };

            bool slotChanged = !string.IsNullOrWhiteSpace(dto.SlotId) && dto.SlotId != booking.SlotId;
            if (slotChanged)
            {
                var newSlot = await _ctx.TestSlots.FirstOrDefaultAsync(s => s.Id == dto.SlotId);
                if (newSlot is null)
                    return new ServiceResponse<bool> { Success = false, Message = "New TestSlot not found." };
                if (newSlot.IsBooked)
                    return new ServiceResponse<bool> { Success = false, Message = "New TestSlot is already booked." };

                // free old
                if (booking.Slot is not null)
                {
                    booking.Slot.IsBooked = false;
                    booking.Slot.BookedAt = null;
                    booking.Slot.BookedByUserId = null;
                }

                booking.SlotId = dto.SlotId!;
                newSlot.IsBooked = true;
                newSlot.BookedByUserId = booking.CustomerId;
                newSlot.BookedAt = DateTimeOffset.UtcNow;
            }

            booking.Status = dto.Status;
            booking.ResultUrl = dto.ResultUrl;

            await _ctx.SaveChangesAsync();
            return new ServiceResponse<bool>
            { Data = true, Success = true, Message = "Updated successfully" };
        }

        /* ---------------- DELETE ---------------- */
        public async Task<ServiceResponse<bool>> DeleteAsync(string id)
        {
            var booking = await _repo.GetByIdAsync(id);
            if (booking is null)
                return new ServiceResponse<bool> { Success = false, Message = "TestBooking not found." };

            // free its slot
            var slot = await _ctx.TestSlots.FirstOrDefaultAsync(s => s.Id == booking.SlotId);
            if (slot is not null)
            {
                slot.IsBooked = false;
                slot.BookedAt = null;
                slot.BookedByUserId = null;
            }

            _repo.Delete(booking);
            await _repo.SaveChangesAsync();

            return new ServiceResponse<bool>
            { Data = true, Success = true, Message = "Deleted successfully" };
        }

        /* ---------------- READ ---------------- */
        public async Task<ServiceResponse<TestBookingDto>> GetByIdAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity is null
                ? new ServiceResponse<TestBookingDto>
                { Success = false, Message = "TestBooking not found." }
                : new ServiceResponse<TestBookingDto>
                { Data = _mapper.Map<TestBookingDto>(entity), Success = true, Message = "Retrieved successfully" };
        }

        public async Task<ServiceResponse<PaginatedList<TestBookingDto>>> GetAllAsync(int page, int size)
        {
            var q = _repo.Query().OrderByDescending(t => t.CreatedTime);
            var paged = await PaginatedList<TestBooking>.CreateAsync(q, page, size);
            var result = new PaginatedList<TestBookingDto>(
                            paged.Items.Select(_mapper.Map<TestBookingDto>).ToList(),
                            paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<TestBookingDto>>
            { Data = result, Success = true, Message = "Test bookings retrieved successfully" };
        }

        public async Task<ServiceResponse<PaginatedList<TestBookingDto>>> SearchAsync(
            string? status, string? customerId, int page, int size)
        {
            var paged = await _repo.SearchAsync(status, customerId, page, size);
            var result = new PaginatedList<TestBookingDto>(
                            paged.Items.Select(_mapper.Map<TestBookingDto>).ToList(),
                            paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<TestBookingDto>>
            { Data = result, Success = true, Message = "Search completed successfully" };
        }

        public async Task<ServiceResponse<PaginatedList<TestBookingDto>>>
            GetByUserAsync(string customerId, int page, int size)
        {
            var paged = await _repo.GetByUserAsync(customerId, page, size);

            var items = paged.Items
                             .Select(_mapper.Map<TestBookingDto>)
                             .ToList();

            var result = new PaginatedList<TestBookingDto>(items, paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<TestBookingDto>>
            {
                Data = result,
                Success = true,
                Message = "Lấy lịch sử booking thành công"
            };
        }
    }
}
