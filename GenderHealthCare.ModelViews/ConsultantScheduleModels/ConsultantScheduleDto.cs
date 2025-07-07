namespace GenderHealthCare.ModelViews.ConsultantScheduleModels
{
    public class ConsultantScheduleDto
    {
        public string Id { get; set; } = default!;
        public DateTime AvailableDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string ConsultantId { get; set; } = default!;
        public string ConsultantName { get; set; } = default!;
    }
}
