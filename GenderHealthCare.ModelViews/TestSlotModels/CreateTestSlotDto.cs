namespace GenderHealthCare.ModelViews.TestSlotModels
{
    public class CreateTestSlotDto
    {
        public DateTime TestDate { get; set; }
        public TimeSpan SlotStart { get; set; }
        public TimeSpan SlotEnd { get; set; }
        public string HealthTestId { get; set; }
    }
}