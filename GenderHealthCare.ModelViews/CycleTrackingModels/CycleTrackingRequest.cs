namespace GenderHealthCare.ModelViews.CycleTrackingModels
{
    public class CycleTrackingRequest
    {
        public DateTime StartDate { get; set; }
        public int? CycleLength { get; set; }
        public int? PeriodLength { get; set; }
        public string? Notes { get; set; }
    }
}
