using AutoMapper;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Enums;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.TestBookingModel;
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
        private readonly GenderHealthCareDbContext _context;  // ⭐️
        private readonly IMapper _mapper;

        public TestBookingService(ITestBookingRepository repo,
                                  GenderHealthCareDbContext context,
                                  IMapper mapper)
        {
            _repo = repo;
            _context = context;
            _mapper = mapper;
        }


        public async Task<string> CreateAsync(CreateTestBookingDto dto)
        {
            // 1. Lấy slot + kiểm tra đã được đặt chưa
            var slot = await _context.TestSlots
                .FirstOrDefaultAsync(s => s.Id == dto.SlotId)
                ?? throw new KeyNotFoundException("TestSlot not found");

            if (slot.IsBooked)
                throw new InvalidOperationException("This slot is already booked");

            // 2. Tạo booking
            var booking = _mapper.Map<TestBooking>(dto);
            booking.Status = TestBookingStatus.Pending.ToString();

            await _repo.AddAsync(booking);

            // 3. Cập‑nhật slot
            slot.IsBooked = true;
            slot.BookedByUserId = dto.CustomerId;
            slot.BookedAt = DateTimeOffset.UtcNow;

            // 4. Lưu thay đổi (cùng transaction DbContext)
            await _repo.SaveChangesAsync();

            return booking.Id;
        }

        public async Task UpdateAsync(string id, UpdateTestBookingDto dto)
        {
            var booking = await _context.TestBookings
                .Include(b => b.Slot)
                .FirstOrDefaultAsync(b => b.Id == id)
                ?? throw new KeyNotFoundException("TestBooking not found");

            bool isSlotChanged = dto.SlotId != null && dto.SlotId != booking.SlotId;

            if (isSlotChanged)
            {
                // 1. Giải phóng slot cũ
                var oldSlot = await _context.TestSlots.FirstOrDefaultAsync(s => s.Id == booking.SlotId);
                if (oldSlot != null)
                {
                    oldSlot.IsBooked = false;
                    oldSlot.BookedByUserId = null;
                    oldSlot.BookedAt = null;
                }

                // 2. Kiểm tra slot mới
                var newSlot = await _context.TestSlots
                    .FirstOrDefaultAsync(s => s.Id == dto.SlotId)
                    ?? throw new KeyNotFoundException("New TestSlot not found");

                if (newSlot.IsBooked)
                    throw new InvalidOperationException("New TestSlot is already booked");

                // 3. Gán slot mới cho booking
                booking.SlotId = dto.SlotId!;
                newSlot.IsBooked = true;
                newSlot.BookedByUserId = booking.CustomerId;
                newSlot.BookedAt = DateTimeOffset.UtcNow;
            }

            // 4. Cập nhật các field khác (Status, ResultUrl)
            booking.Status = dto.Status;
            booking.ResultUrl = dto.ResultUrl;

            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("TestBooking not found");

            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task<TestBookingDto?> GetByIdAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<TestBookingDto>(entity);
        }

        public async Task<PaginatedList<TestBookingDto>> GetAllAsync(int page, int size)
        {
            var query = _repo.Query().OrderByDescending(t => t.CreatedTime);
            var paged = await PaginatedList<TestBooking>.CreateAsync(query, page, size);
            return new PaginatedList<TestBookingDto>(
                paged.Items.Select(_mapper.Map<TestBookingDto>).ToList(),
                paged.TotalCount, page, size);
        }

        public async Task<PaginatedList<TestBookingDto>> SearchAsync(string? status, string? customerId, int page, int size)
        {
            var paged = await _repo.SearchAsync(status, customerId, page, size);
            return new PaginatedList<TestBookingDto>(
                paged.Items.Select(_mapper.Map<TestBookingDto>).ToList(),
                paged.TotalCount, page, size);
        }
    }
}
