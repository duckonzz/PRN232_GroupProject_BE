namespace GenderHealthCare.ModelViews.ConsultationModels
{
    public class ConsultationResponse
    {
        public string Id { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; }          // Pending / Confirmed / Completed / Cancelled
        public string? Result { get; set; }

        // Slot Info
        public string SlotId { get; set; }
        public DateTime AvailableDate { get; set; } // Slot.Schedule.AvailableDate
        public TimeSpan SlotStart { get; set; }
        public TimeSpan SlotEnd { get; set; }

        // Consultant Info
        public string ConsultantId { get; set; }
        public string ConsultantFullName { get; set; }

        // User Info
        public string UserId { get; set; }
        public string UserFullName { get; set; }
    }
}
