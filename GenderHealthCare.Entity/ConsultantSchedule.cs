using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class ConsultantSchedule : BaseEntity
    {
        public DateTime AvailableDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string ConsultantId { get; set; }

        public Consultant Consultant { get; set; }
        public ICollection<AvailableSlot> Slots { get; set; }
    }
}
