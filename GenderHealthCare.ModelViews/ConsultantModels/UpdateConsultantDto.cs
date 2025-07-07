namespace GenderHealthCare.ModelViews.ConsultantModels
{
    public class UpdateConsultantDto
    {
        // ----- User -----
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string Role { get; set; }
        public bool IsCycleTrackingOn { get; set; }
        public string? NewPassword { get; set; }

        // ----- Consultant -----
        public string Degree { get; set; }
        public int ExperienceYears { get; set; }
        public string? Bio { get; set; }
    }
}
