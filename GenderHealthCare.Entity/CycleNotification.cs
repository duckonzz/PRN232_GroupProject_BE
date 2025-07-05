using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class CycleNotification : BaseEntity
    {
        public string NotificationType { get; set; } // e.g., UpcomingPeriod, OvulationDay, FertileWindow
        public DateTime NotificationDate { get; set; }
        public bool IsSent { get; set; }
        public DateTimeOffset? SentAt { get; set; }
        public string? Message { get; set; }
        public string UserId { get; set; }
        public string ReproductiveCycleId { get; set; }

        public User User { get; set; }
        public ReproductiveCycle ReproductiveCycle { get; set; }
    }
}
