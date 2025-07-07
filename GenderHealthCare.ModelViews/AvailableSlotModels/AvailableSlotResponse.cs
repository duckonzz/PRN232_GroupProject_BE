namespace GenderHealthCare.ModelViews.AvailableSlotModels
{
    public class AvailableSlotResponse
    {
        public string Id { get; set; }
        public TimeSpan SlotStart { get; set; }
        public TimeSpan SlotEnd { get; set; }
        public bool IsBooked { get; set; }
        public DateTimeOffset? BookedAt { get; set; }
        public string ScheduleId { get; set; }
        public DateTime AvailableDate { get; set; }
        public string ConsultantId { get; set; }
    }
}
