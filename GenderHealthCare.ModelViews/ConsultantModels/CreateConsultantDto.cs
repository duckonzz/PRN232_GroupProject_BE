﻿namespace GenderHealthCare.ModelViews.ConsultantModels
{
    public class CreateConsultantDto
    {
        // ----- User -----
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "Consultant";       // mặc định
        public bool IsCycleTrackingOn { get; set; } = false;   // tùy chỉnh

        // ----- Consultant -----
        public string Degree { get; set; }
        public int ExperienceYears { get; set; }
        public string? Bio { get; set; }
    }
}
