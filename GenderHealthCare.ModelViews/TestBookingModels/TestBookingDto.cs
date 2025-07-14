namespace GenderHealthCare.ModelViews.TestBookingModels
{
    public class TestBookingDto
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string? ResultUrl { get; set; }
        public string SlotId { get; set; }
        public string CustomerId { get; set; }

        // User Info
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }

        // HealthTest Info
        public string? HealthTestId { get; set; }
        public string? HealthTestName { get; set; }
        public decimal? HealthTestPrice { get; set; }

        // Booking date
        public DateTime TestDate { get; set; }
    }
}
