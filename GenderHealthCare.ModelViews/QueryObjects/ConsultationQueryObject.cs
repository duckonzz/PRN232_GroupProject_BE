namespace GenderHealthCare.ModelViews.QueryObjects
{
    public class ConsultationQueryObject : BaseQueryObject
    {
        public string? UserId { get; set; }
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public TimeSpan? FromTime { get; set; }
        public TimeSpan? ToTime { get; set; }
    }
}
