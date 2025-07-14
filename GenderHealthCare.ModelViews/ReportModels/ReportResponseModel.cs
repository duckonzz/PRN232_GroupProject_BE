namespace GenderHealthCare.ModelViews.ReportModels
{
    public class ReportResponseModel
    {
        public string Id { get; set; }
        public string ReportType { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string Notes { get; set; }
        public string GeneratedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
    }

}
