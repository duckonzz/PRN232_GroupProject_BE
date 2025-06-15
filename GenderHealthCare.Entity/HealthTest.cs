using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class HealthTest : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public ICollection<TestSlot> Slots { get; set; }
        public ICollection<HealthTestSchedule> HealthTestSchedules { get; set; }
    }
}
