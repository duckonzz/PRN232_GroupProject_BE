namespace GenderHealthCare.ModelViews.StatisticsModels
{
    public class ServiceRevenueItem
    {
        public string HealthTestId { get; set; }
        public string HealthTestName { get; set; }
        public long TotalRevenue { get; set; }
        public int TransactionCount { get; set; }
    }
}
