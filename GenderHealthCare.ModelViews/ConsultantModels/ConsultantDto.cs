namespace GenderHealthCare.ModelViews.ConsultantModels
{
    public class ConsultantDto
    {
        // Consultant
        public string Id { get; set; }
        public string? Degree { get; set; }
        public int ExperienceYears { get; set; }
        public string? Bio { get; set; }

        // User
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string Role { get; set; }
        public bool IsCycleTrackingOn { get; set; }
    }
}
