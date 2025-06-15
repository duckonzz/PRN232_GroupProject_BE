using GenderHealthCare.Core.Enums;
using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class Consultation : BaseEntity
    {
        public string? Reason { get; set; }
        public string Status { get; set; } = ConsultationStatus.Pending.ToString();
        public string? Result { get; set; }
        public string SlotId { get; set; }
        public string UserId { get; set; }
        public string ConsultantId { get; set; }

        public AvailableSlot Slot { get; set; }
        public User User { get; set; }
        public Consultant Consultant { get; set; }
    }
}
