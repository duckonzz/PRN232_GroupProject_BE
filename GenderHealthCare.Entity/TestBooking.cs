using GenderHealthCare.Core.Enums;
using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class TestBooking : BaseEntity
    {
        public string Status { get; set; } = TestBookingStatus.Pending.ToString();
        public string? ResultUrl { get; set; }
        public string SlotId { get; set; }
        public string CustomerId { get; set; }
        public TestSlot Slot { get; set; }
        public User Customer { get; set; }
    }
}
