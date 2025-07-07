namespace GenderHealthCare.ModelViews.ConsultantScheduleModels
{
    public class UpdateConsultantScheduleDto
    {
        public DateTime AvailableDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string ConsultantId { get; set; } = default!;
    }
}
