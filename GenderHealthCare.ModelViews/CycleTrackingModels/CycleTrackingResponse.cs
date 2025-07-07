namespace GenderHealthCare.ModelViews.CycleTrackingModels
{
    public class CycleTrackingResponse
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CycleLength { get; set; }
        public int PeriodLength { get; set; }
        public string? Notes { get; set; }

        public DateTime UpcomingPeriod { get; set; }
        public DateTime OvulationDate { get; set; }
        public DateTime FertileWindowStart { get; set; }
        public DateTime FertileWindowEnd { get; set; }
    }
}
