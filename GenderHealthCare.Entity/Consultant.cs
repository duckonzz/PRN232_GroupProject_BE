using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class Consultant : BaseEntity
    {
        public string? Degree { get; set; }
        public int ExperienceYears { get; set; }
        public string? Bio { get; set; }
        public string UserId { get; set; }

        public User User { get; set; }
        public ICollection<ConsultantSchedule> Schedules { get; set; }
        public ICollection<Consultation> Consultations { get; set; }
        public ICollection<QAThread> QAThreads { get; set; }
    }
}
