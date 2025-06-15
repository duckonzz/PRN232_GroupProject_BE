using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class Feedback : BaseEntity
    {
        public string TargetType { get; set; } // e.g., Consultation, HealthTest (enum)
        public string TargetId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string UserId { get; set; }

        public User User { get; set; }
    }
}
