using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class TestSlot : BaseEntity
    {
        public DateTime TestDate { get; set; }
        public TimeSpan SlotStart { get; set; }
        public TimeSpan SlotEnd { get; set; }
        public bool IsBooked { get; set; }
        public string? BookedByUserId { get; set; }
        public DateTimeOffset? BookedAt { get; set; }
        public string HealthTestId { get; set; }

        public HealthTest HealthTest { get; set; }
        public User? BookedByUser { get; set; }
        public ICollection<TestBooking> TestBookings { get; set; }
    }
}
