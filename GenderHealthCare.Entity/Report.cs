using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class Report : BaseEntity
    {
        public string ReportType { get; set; } // Week, Month...
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string Content { get; set; } // JSON format
        public string? Notes { get; set; }
        public string GeneratedByUserId { get; set; }

        public User GeneratedByUser { get; set; }
    }
}
