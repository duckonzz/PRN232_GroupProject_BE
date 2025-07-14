using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class HealthTestSchedule : BaseEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan SlotStart { get; set; }
        public TimeSpan SlotEnd { get; set; }
        public int SlotDurationInMinutes { get; set; } = 60;

        public string DaysOfWeek { get; set; } // CSV: "Mon,Wed,Fri"
        public string HealthTestId { get; set; }
        public HealthTest HealthTest { get; set; }

        public ICollection<TestSlot> TestSlots { get; set; }
    }
}
