using AutoMapper;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Enums;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.TestSlotModels;
using GenderHealthCare.Repositories.Base;
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
        private readonly GenderHealthCareDbContext _ctx;

        public TestSlotService(ITestSlotRepository repo,
            GenderHealthCareDbContext ctx, IMapper mapper)
        {
            _repo = repo;
            _ctx = ctx;
            _mapper = mapper;
        }

        /* ---------------- CREATE ---------------- */
        public async Task<ServiceResponse<CreateTestSlotResultDto>>
            CreateAsync(CreateTestSlotDto dto)
        {
            /* 0. validate thời gian (giữ nguyên nhưng rút gọn) */
            if (dto.SlotEnd <= dto.SlotStart)
                return Bad("SlotEnd must be later than SlotStart.");

            var today = DateTime.Today;
            var maxDate = today.AddMonths(1);
            if (dto.TestDate.Date < today)
                return Bad("TestDate cannot be in the past.");
            if (dto.TestDate.Date > maxDate)
                return Bad("TestDate cannot be more than one month in the future.");

            /* 1. Overlap check */
            bool overlap = await _repo.Query().AnyAsync(s =>
                s.HealthTestId == dto.HealthTestId &&
                s.TestDate.Date == dto.TestDate.Date &&
                dto.SlotStart < s.SlotEnd &&
                dto.SlotEnd > s.SlotStart);

            if (overlap) return Bad("Another slot overlaps this time range.");

            /* 2. Nếu truyền customerId thì phải tồn tại User */
            if (dto.CustomerId != null &&
                !await _ctx.Users.AnyAsync(u => u.Id == dto.CustomerId))
                return Bad("Customer not found.");

            /* 3. Transaction: tạo Slot (+ Booking nếu có customer) */
            await using var tx = await _ctx.Database.BeginTransactionAsync();

            try
            {
                var slot = _mapper.Map<TestSlot>(dto);
                slot.IsBooked = dto.CustomerId != null;
                slot.BookedByUserId = dto.CustomerId;
                slot.BookedAt = dto.CustomerId != null ? DateTimeOffset.UtcNow : null;

                await _repo.AddAsync(slot);

                string? bookingId = null;
                if (dto.CustomerId != null)
                {
                    var booking = new TestBooking
                    {
                        SlotId = slot.Id,
                        CustomerId = dto.CustomerId,
                        Status = TestBookingStatus.Pending.ToString()
                    };
                    await _ctx.TestBookings.AddAsync(booking);
                    bookingId = booking.Id;
                }

                await _repo.SaveChangesAsync();
                await tx.CommitAsync();

                var result = new CreateTestSlotResultDto
                {
                    SlotId = slot.Id,
                    BookingId = bookingId
                };

                return new ServiceResponse<CreateTestSlotResultDto>
                {
                    Data = result,
                    Success = true,
                    Message = dto.CustomerId != null
                              ? "Slot created & booked successfully."
                              : "Slot created successfully."
                };
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Bad("Database error. Please retry.");
            }

            /* local helper */
            ServiceResponse<CreateTestSlotResultDto> Bad(string msg) =>
                new() { Success = false, Message = msg };
        }

        /* ---------------- UPDATE ---------------- */
        public async Task<ServiceResponse<bool>>
    UpdateAsync(string id, UpdateTestSlotDto dto)
        {
            /* 0. Lấy slot hiện tại */
            var slot = await _repo.GetByIdAsync(id);
            if (slot == null) return Fail("Test slot not found.");

            /* -------- 1. VALIDATE KHUNG GIỜ & CHỒNG LẤN (dù book hay không) -------- */
            if (dto.SlotEnd <= dto.SlotStart)
                return Fail("SlotEnd must be later than SlotStart.");

            var today = DateTime.Today;
            if (dto.TestDate.Date < today)
                return Fail("TestDate cannot be in the past.");
            if (dto.TestDate.Date > today.AddMonths(1))
                return Fail("TestDate cannot be more than one month in the future.");

            /* Không kiểm tra overlap nếu slot đã booked & client không đổi giờ */
            bool timeChanged = slot.TestDate != dto.TestDate ||
                               slot.SlotStart != dto.SlotStart ||
                               slot.SlotEnd != dto.SlotEnd ||
                               slot.HealthTestId != dto.HealthTestId;

            if (timeChanged)
            {
                // chỉ cho đổi thời gian khi slot CHƯA booked
                if (slot.IsBooked)
                    return Fail("This slot is already booked; you cannot change its time.");

                bool overlap = await _repo.Query().AnyAsync(s =>
                    s.Id != slot.Id &&
                    s.HealthTestId == dto.HealthTestId &&
                    s.TestDate.Date == dto.TestDate.Date &&
                    dto.SlotStart < s.SlotEnd &&
                    dto.SlotEnd > s.SlotStart);

                if (overlap) return Fail("Another slot overlaps this time range.");
            }

            /* -------- 2. ÁP DỤNG UPDATE THUỘC TÍNH CƠ BẢN -------- */
            slot.TestDate = dto.TestDate;
            slot.SlotStart = dto.SlotStart;
            slot.SlotEnd = dto.SlotEnd;
            slot.HealthTestId = dto.HealthTestId;

            /* -------- 3. XỬ LÝ BOOKING (nếu client yêu cầu) -------- */
            if (dto.IsBooked && !string.IsNullOrWhiteSpace(dto.BookedByUserId))
            {
                // A. Nếu slot chưa booked → tạo booking mới
                if (!slot.IsBooked)
                {
                    // 3A‑1. kiểm tra user
                    var userExists = await _ctx.Users.AnyAsync(u => u.Id == dto.BookedByUserId);
                    if (!userExists) return Fail("Customer not found.");

                    // 3A‑2. đánh dấu slot đã đặt
                    slot.IsBooked = true;
                    slot.BookedByUserId = dto.BookedByUserId;
                    slot.BookedAt = DateTimeOffset.UtcNow;

                    // 3A‑3. sinh record TestBooking
                    var booking = new TestBooking
                    {
                        SlotId = slot.Id,
                        CustomerId = dto.BookedByUserId,
                        Status = TestBookingStatus.Pending.ToString()
                    };
                    await _ctx.TestBookings.AddAsync(booking);
                }
                // B. Nếu slot đã booked rồi nhưng đổi user → không cho phép ở đây
                else if (slot.BookedByUserId != dto.BookedByUserId)
                {
                    return Fail("Slot already booked by another user. Cannot change booking here.");
                }
            }
            else
            {
                /* Nếu client gửi IsBooked=false => chỉ cho khi slot hiện cũng chưa booked */
                if (!dto.IsBooked && slot.IsBooked)
                    return Fail("You cannot un-book this slot via update endpoint.");
            }

            /* -------- 4. SAVE -------- */
            await _repo.SaveChangesAsync();
            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Updated successfully."
            };

            /* helper */
            ServiceResponse<bool> Fail(string m) => new() { Success = false, Message = m };
        }


        /* ---------------- DELETE ---------------- */
        public async Task<ServiceResponse<bool>> DeleteAsync(string id)
        {
            var slot = await _repo.GetByIdAsync(id);
            if (slot is null)
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Test slot not found."
                };

            /* ==== NEW: chặn xóa khi slot đã được đặt ==== */
            if (slot.IsBooked)
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Cannot delete: this slot has already been booked."
                };

            /* Nếu tới đây => slot đang trống, cho phép xóa */
            _repo.Delete(slot);
            await _repo.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Test slot deleted successfully."
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
