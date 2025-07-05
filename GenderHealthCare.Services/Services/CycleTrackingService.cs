using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Constants;
using GenderHealthCare.Core.Enums;
using GenderHealthCare.Core.Exceptions;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.CycleTrackingModels;
using GenderHealthCare.Services.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GenderHealthCare.Services.Services
{
    public class CycleTrackingService : ICycleTrackingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;

        public CycleTrackingService(IUnitOfWork unitOfWork, IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
        }

        public async Task<CycleTrackingResponse> CreateCycleAsync(string userId, CycleTrackingRequest request)
        {
            await ValidateCycleTrackingOn(userId);
            var cycleRepo = _unitOfWork.GetRepository<ReproductiveCycle>();
            var previousCycle = await cycleRepo.Entities
                .Where(c => c.UserId == userId && !c.DeletedTime.HasValue)
                .OrderByDescending(c => c.StartDate)
                .FirstOrDefaultAsync();

            if (previousCycle != null && request.StartDate <= previousCycle.EndDate)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Start date must be after the previous cycle's end date");
            }

            var cycleLength = request.CycleLength ?? previousCycle?.CycleLength ?? 28;
            var periodLength = request.PeriodLength ?? previousCycle?.PeriodLength ?? 5;

            var cycle = new ReproductiveCycle
            {
                UserId = userId,
                StartDate = request.StartDate,
                EndDate = request.StartDate.AddDays(periodLength - 1),
                CycleLength = cycleLength,
                PeriodLength = periodLength,
                Notes = request.Notes,
            };

            await cycleRepo.InsertAsync(cycle);
            await _unitOfWork.SaveAsync();

            await GenerateNotificationsAsync(userId, cycle);

            return cycle.ToCycleTrackingDto();
        }

        public async Task DeleteCycleAsync(string cycleId, string userId)
        {
            await ValidateCycleTrackingOn(userId);
            var cycleRepo = _unitOfWork.GetRepository<ReproductiveCycle>();
            var cycle = await cycleRepo.Entities
                .FirstOrDefaultAsync(c => c.Id == cycleId && c.UserId == userId)
                ?? throw new Exception("Cycle not found");

            cycle.DeletedTime = CoreHelper.SystemTimeNow;

            cycleRepo.Update(cycle);
            await _unitOfWork.SaveAsync();
        }

        public async Task<CycleTrackingResponse> GetCycleByIdAsync(string cycleId, string userId)
        {
            await ValidateCycleTrackingOn(userId);
            var cycleRepo = _unitOfWork.GetRepository<ReproductiveCycle>();
            var cycle = await cycleRepo.Entities
                .FirstOrDefaultAsync(c => c.Id == cycleId && c.UserId == userId && !c.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cycle not found");

            return cycle.ToCycleTrackingDto();
        }

        public async Task<IEnumerable<CycleTrackingResponse>> GetCyclesAsync(string userId)
        {
            await ValidateCycleTrackingOn(userId);
            var cycleRepo = _unitOfWork.GetRepository<ReproductiveCycle>();
            var cycles = await cycleRepo.Entities
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();

            return cycles.ToCycleTrackingDtoList();
        }

        public async Task UpdateUserCycleTrackingAsync(string userId, bool isEnabled)
        {
            var user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Id == userId && !u.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");

            if (!isEnabled)
            {
                var notifyRepo = _unitOfWork.GetRepository<CycleNotification>();
                var pendingNotifications = await notifyRepo.Entities
                    .Where(n => n.UserId == userId && !n.IsSent && !n.DeletedTime.HasValue)
                    .ToListAsync();

                pendingNotifications.ForEach(n => n.DeletedTime = CoreHelper.SystemTimeNow);

                notifyRepo.UpdateRange(pendingNotifications);
            }

            user.IsCycleTrackingOn = isEnabled == true;
            user.LastUpdatedTime = CoreHelper.SystemTimeNow;

            _unitOfWork.GetRepository<User>().Update(user);
            await _unitOfWork.SaveAsync();
        }

        #region Shared private methods
        private async Task GenerateNotificationsAsync(string userId, ReproductiveCycle cycle)
        {
            var notifyRepo = _unitOfWork.GetRepository<CycleNotification>();
            var notifications = new List<CycleNotification>();

            var upcomingPeriodDate = cycle.StartDate.AddDays(cycle.CycleLength - 3);
            var ovulationDate = cycle.StartDate.AddDays(cycle.CycleLength - 14);

            var fertileWindowStart = ovulationDate.AddDays(-3);
            var fertileWindowEnd = ovulationDate.AddDays(1);

            notifications.Add(new CycleNotification
            {
                UserId = userId,
                ReproductiveCycleId = cycle.Id,
                NotificationType = NotificationType.UpcomingPeriod.ToString(),
                NotificationDate = upcomingPeriodDate,
                Message = "Kỳ kinh tiếp theo sẽ bắt đầu trong 3 ngày nữa",
            });

            notifications.Add(new CycleNotification
            {
                UserId = userId,
                ReproductiveCycleId = cycle.Id,
                NotificationType = NotificationType.Ovulation.ToString(),
                NotificationDate = ovulationDate,
                Message = "Hôm nay là ngày rụng trứng của bạn",
            });

            for (var date = fertileWindowStart; date <= fertileWindowEnd; date = date.AddDays(1))
            {
                notifications.Add(new CycleNotification
                {
                    UserId = userId,
                    ReproductiveCycleId = cycle.Id,
                    NotificationType = NotificationType.FertileWindow.ToString(),
                    NotificationDate = date,
                    Message = $"Hôm nay ({date:dd/MM}) là ngày dễ thụ thai của bạn"
                });
            }

            await notifyRepo.InsertRangeAsync(notifications);
            await _unitOfWork.SaveAsync();
        }

        private async Task ValidateCycleTrackingOn(string userId)
        {
            var user = await _authenticationService.GetUserByIdAsync(userId);
            if (!user.IsCycleTrackingOn)
                throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Cycle tracking is disabled for this user.");
        }
        #endregion
    }
}
