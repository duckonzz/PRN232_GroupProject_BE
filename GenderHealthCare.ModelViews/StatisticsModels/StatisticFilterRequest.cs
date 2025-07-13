namespace GenderHealthCare.ModelViews.StatisticsModels
{
    public class StatisticFilterRequest
    {
        public int Year { get; set; }  
        public int? Month { get; set; }

        // Optional filters
        public string? ConsultantId { get; set; }
        public string? HealthTestId { get; set; }
        public string? Status { get; set; }
    }
}
