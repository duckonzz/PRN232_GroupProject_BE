namespace GenderHealthCare.ModelViews.StatisticsModels
{
    public class RevenueStatisticsResponse
    {
        public long TotalRevenue { get; set; }
        public int TotalTransactions { get; set; }
        public int SuccessfulTransactions { get; set; }
        public int FailedTransactions { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<ServiceRevenueItem> RevenueByService { get; set; } = new();
        public List<MonthlyRevenueItem> RevenueByMonth { get; set; } = new();
    }
}
