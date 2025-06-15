using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class AvailableSlot : BaseEntity
    {
        public TimeSpan SlotStart { get; set; }
        public TimeSpan SlotEnd { get; set; }
        public bool IsBooked { get; set; }
        public string? BookedByUserId { get; set; }
        public DateTimeOffset? BookedAt { get; set; }
        public string ScheduleId { get; set; }

        public ConsultantSchedule Schedule { get; set; }
        public User? BookedByUser { get; set; }
        public ICollection<Consultation> Consultations { get; set; }
    }
}
