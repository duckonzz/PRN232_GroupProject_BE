using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GenderHealthCare.Services.BackgroundJobs
{
    public class CycleNotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CycleNotificationBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

        public CycleNotificationBackgroundService(IServiceProvider serviceProvider, ILogger<CycleNotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Cycle Notification Background Service is starting...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                        var templateBuilder = scope.ServiceProvider.GetRequiredService<IEmailTemplateBuilder>();
                        var cycleTrackingService = scope.ServiceProvider.GetRequiredService<ICycleTrackingService>();

                        await AutoGenerateNextCyclesIfNeeded(unitOfWork, cycleTrackingService);
                        await ProcessCycleNotifications(unitOfWork, emailService, templateBuilder);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing cycle notifications.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Cycle Notification Background Service is stopping...");
        }

        private async Task ProcessCycleNotifications(IUnitOfWork unitOfWork, IEmailService emailService, IEmailTemplateBuilder templateBuilder)
        {
            var notifyRepo = unitOfWork.GetRepository<CycleNotification>();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var notifications = await notifyRepo.Entities
                .Where(n => !n.IsSent && n.NotificationDate >= today && n.NotificationDate < tomorrow)
                .Include(n => n.User)
                .ToListAsync();

            _logger.LogInformation("Found {Count} notifications scheduled for {Date}", notifications.Count, today.ToShortDateString());

            foreach (var notify in notifications)
            {
                if (notify.User == null || !notify.User.IsCycleTrackingOn)
                {
                    _logger.LogWarning("Skipping notification ID {Id} due to missing or disabled user", notify.Id);
                    continue;
                }

                _logger.LogInformation("Sending email to {Email} for notification type {Type}", notify.User.Email, notify.NotificationType);

                var templateName = notify.NotificationType switch
                {
                    "UpcomingPeriod" => "UpcomingPeriod",
                    "Ovulation" => "Ovulation",
                    "FertileWindow" => "FertileWindow",
                    _ => throw new Exception("Unsupported notification type")
                };

                var htmlContent = await templateBuilder.BuildAsync(templateName, new
                {
                    FullName = notify.User.FullName,
                    NotificationDate = notify.NotificationDate
                });

                await emailService.SendEmailAsync(
                    toEmail: notify.User.Email,
                    subject: "Thông báo theo dõi chu kỳ",
                    body: htmlContent
                );

                _logger.LogInformation("Email sent to {Email}", notify.User.Email);

                notify.IsSent = true;
                notify.SentAt = CoreHelper.SystemTimeNow;
            }

            notifyRepo.UpdateRange(notifications);
            await unitOfWork.SaveAsync();
            _logger.LogInformation("Notification statuses updated in DB.");
        }

        private async Task AutoGenerateNextCyclesIfNeeded(IUnitOfWork unitOfWork, ICycleTrackingService cycleTrackingService)
        {
            var userRepo = unitOfWork.GetRepository<User>();
            var cycleRepo = unitOfWork.GetRepository<ReproductiveCycle>();

            var users = await userRepo.Entities
                .Where(u => u.IsCycleTrackingOn && !u.DeletedTime.HasValue)
                .ToListAsync();

            foreach (var user in users)
            {
                var lastCycle = await cycleRepo.Entities
                    .Where(c => c.UserId == user.Id && !c.DeletedTime.HasValue)
                    .OrderByDescending(c => c.StartDate)
                    .FirstOrDefaultAsync();

                if (lastCycle == null || lastCycle.EndDate >= CoreHelper.SystemTimeNow)
                {
                    continue;
                }

                var nextStartDate = lastCycle.StartDate.AddDays(lastCycle.CycleLength);

                // Nếu đã có chu kỳ thủ công nào (IsAutoGenerated == false) có StartDate >= nextStartDate => bỏ qua tạo tự động
                var hasManualCycle = await cycleRepo.Entities.AnyAsync(c =>
                    c.UserId == user.Id &&
                    !c.DeletedTime.HasValue &&
                    !c.IsAutoGenerated &&
                    c.StartDate >= nextStartDate);

                if (hasManualCycle)
                {
                    continue;
                }

                if (DateTime.Today >= nextStartDate)
                {
                    var newCycle = new ReproductiveCycle
                    {
                        UserId = user.Id,
                        StartDate = nextStartDate,
                        EndDate = nextStartDate.AddDays(lastCycle.PeriodLength - 1),
                        CycleLength = lastCycle.CycleLength,
                        PeriodLength = lastCycle.PeriodLength,
                        Notes = "Chu kỳ được tạo tự động dựa trên dữ liệu thu thập được",
                        IsAutoGenerated = true
                    };

                    await cycleRepo.InsertAsync(newCycle);
                    await unitOfWork.SaveAsync();

                    await cycleTrackingService.GenerateCycleNotificationsAsync(user.Id, newCycle);
                }
            }
        }
    }
}
