using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Constants;
using GenderHealthCare.Core.Enums;
using GenderHealthCare.Core.Exceptions;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.Core.Models;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.ConsultationModels;
using GenderHealthCare.ModelViews.QueryObjects;
using GenderHealthCare.Services.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GenderHealthCare.Services.Services
{
    public class ConsultationService : IConsultationsService

    {
        private readonly IUnitOfWork _unitOfWork;

        public ConsultationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CancelConsultationAsync(string userId, string consultationId, string? reason, string role)
        {
            var consultationRepo = _unitOfWork.GetRepository<Consultation>();
            var consultation = await consultationRepo.Entities
                .Include(c => c.Slot)
                .FirstOrDefaultAsync(c => c.Id == consultationId && !c.DeletedTime.HasValue) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Consultation not found or deleted");

            if(consultation.Status != ConsultationStatus.Pending.ToString() && consultation.Status != ConsultationStatus.Confirmed.ToString())
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Only pending or confirmed consultations can be cancelled");
            }

            var isCustomer = role.Equals(Role.Customer.ToString(), StringComparison.OrdinalIgnoreCase) && consultation.UserId == userId;
            var isConsultant = role.Equals(Role.Consultant.ToString(), StringComparison.OrdinalIgnoreCase) && consultation.ConsultantId == userId;

            if (!isCustomer && !isConsultant)
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You do not have permission to cancel this consultation");
            }

            consultation.Status = ConsultationStatus.Cancelled.ToString();
            consultation.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Optional: thêm Notes = reason nếu cần lưu lý do
            if (!string.IsNullOrWhiteSpace(reason))
            {
                consultation.Reason += $" (Cancelled: {reason.Trim()})";
            }

            // Mở lại slot
            if (consultation.Slot != null)
            {
                consultation.Slot.IsBooked = false;
                consultation.Slot.BookedAt = null;
                consultation.Slot.BookedByUserId = null;
            }

            consultationRepo.Update(consultation);
            await _unitOfWork.SaveAsync();
        }

        public async Task ConfirmConsultationAsync(string consultantId, string consultationId)
        {
            var consultationRepo = _unitOfWork.GetRepository<Consultation>();
            
            var consultation = await consultationRepo.Entities.FirstOrDefaultAsync(x => x.Id == consultationId && x.ConsultantId == consultantId && !x.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Consultation not found");

            if (consultation.Status != ConsultationStatus.Pending.ToString())
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Only pending consultations can be confirmed");
            }

            consultation.Status = ConsultationStatus.Confirmed.ToString();
            consultation.LastUpdatedTime = CoreHelper.SystemTimeNow;

            consultationRepo.Update(consultation);
            await _unitOfWork.SaveAsync();
        }

        public async Task<ConsultationResponse> CreateBookingConsultationAsync(string userId, ConsultationRequest request)
        {
            var slot = await _unitOfWork.GetRepository<AvailableSlot>().Entities
                .Include(s => s.Schedule)
                .FirstOrDefaultAsync(s => s.Id == request.SlotId && !s.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Slot not found");

            if (slot.IsBooked)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Slot already booked");

            var consultation = new Consultation
            {
                SlotId = slot.Id,
                UserId = userId,
                ConsultantId = slot.Schedule.ConsultantId,
                Reason = request.Reason,
            };

            await _unitOfWork.GetRepository<Consultation>().InsertAsync(consultation);

            // Cập nhật trạng thái slot
            slot.IsBooked = true;
            slot.BookedByUserId = userId;
            slot.BookedAt = CoreHelper.SystemTimeNow;

            _unitOfWork.GetRepository<AvailableSlot>().Update(slot);
            await _unitOfWork.SaveAsync();

            // Reload với include đầy đủ
            var fullConsultation = await _unitOfWork.GetRepository<Consultation>().Entities
                .Include(c => c.Slot)
                    .ThenInclude(s => s.Schedule)
                .Include(c => c.Consultant)
                    .ThenInclude(con => con.User)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == consultation.Id);

            return fullConsultation!.ToConsultationDto();
        }

        public async Task<BasePaginatedList<ConsultationResponse>> GetPagedConsultationsAsync(ConsultationQueryObject query)
        {
            var consultationRepo = _unitOfWork.GetRepository<Consultation>();

            var queryable = consultationRepo.Entities
                .AsNoTracking()
                .Include(c => c.Slot)
                    .ThenInclude(s => s.Schedule)
                .Include(c => c.User)
                .Include(c => c.Consultant)
                    .ThenInclude(con => con.User)
                .Where(c => !c.DeletedTime.HasValue);

            if (!string.IsNullOrWhiteSpace(query.UserId))
                queryable = queryable.Where(c => c.UserId == query.UserId);

            if (!string.IsNullOrWhiteSpace(query.ConsultantId))
                queryable = queryable.Where(c => c.ConsultantId == query.ConsultantId);

            if (!string.IsNullOrWhiteSpace(query.Status))
                queryable = queryable.Where(c => c.Status == query.Status);

            if (query.FromDate.HasValue)
                queryable = queryable.Where(c => c.Slot.Schedule.AvailableDate >= query.FromDate.Value.Date);

            if (query.ToDate.HasValue)
                queryable = queryable.Where(c => c.Slot.Schedule.AvailableDate <= query.ToDate.Value.Date);

            if (query.FromTime.HasValue)
                queryable = queryable.Where(c => c.Slot.SlotStart >= query.FromTime.Value);

            if (query.ToTime.HasValue)
                queryable = queryable.Where(c => c.Slot.SlotEnd <= query.ToTime.Value);

            // Sorting
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                queryable = query.SortBy.ToLower() switch
                {
                    "status" => query.IsDescending ? queryable.OrderByDescending(c => c.Status) : queryable.OrderBy(c => c.Status),
                    "date" => query.IsDescending ? queryable.OrderByDescending(c => c.Slot.Schedule.AvailableDate) : queryable.OrderBy(c => c.Slot.Schedule.AvailableDate),
                    "starttime" => query.IsDescending ? queryable.OrderByDescending(c => c.Slot.SlotStart) : queryable.OrderBy(c => c.Slot.SlotStart),
                    _ => queryable.OrderByDescending(c => c.CreatedTime)
                };
            }
            else
            {
                queryable = queryable.OrderByDescending(c => c.CreatedTime);
            }

            // Paging
            var totalCount = await queryable.CountAsync();
            var pagedItems = await queryable
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var dtoList = pagedItems.Select(c => c.ToConsultationDto()).ToList();

            return new BasePaginatedList<ConsultationResponse>(dtoList, totalCount, query.PageIndex, query.PageSize);
        }


        public async Task UpdateConsultationResultAsync(string consultantId, string consultationId, string result)
        {
            var consultationRepo = _unitOfWork.GetRepository<Consultation>();
            var consultation = await consultationRepo.Entities
                .FirstOrDefaultAsync(x => x.Id == consultationId && x.ConsultantId == consultantId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Consultation not found");

            if (consultation.Status == ConsultationStatus.Cancelled.ToString() || consultation.Status == ConsultationStatus.Pending.ToString())
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Cannot update result in current status");

            consultation.Result = result;
            consultation.Status = ConsultationStatus.Completed.ToString();

            consultationRepo.Update(consultation);
            await _unitOfWork.SaveAsync();
        }

    }
}
