namespace GenderHealthCare.ModelViews.QueryObjects
{
    public class AvailableSlotQueryObject : BaseQueryObject
    {
        public string? ConsultantId { get; set; }

        public string? ScheduleId { get; set; }

        public DateTime? Date { get; set; }

        public TimeSpan? FromTime { get; set; }

        public TimeSpan? ToTime { get; set; }

        public bool? IsBooked { get; set; }
    }
}
