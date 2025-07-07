namespace GenderHealthCare.ModelViews.TestSlotModels
{
    public class TestSlotDto
    {
        public string Id { get; set; }
        public DateTime TestDate { get; set; }
        public TimeSpan SlotStart { get; set; }
        public TimeSpan SlotEnd { get; set; }
        public bool IsBooked { get; set; }
        public string? BookedByUserId { get; set; }
        public DateTimeOffset? BookedAt { get; set; }
        public string HealthTestId { get; set; }
    }
}