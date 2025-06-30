using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class User : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string Role { get; set; }
        public bool IsCycleTrackingOn { get; set; }

        public Consultant? ConsultantProfile { get; set; }
        public ICollection<Consultation> Consultations { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<TestSlot> TestSlotsBooked { get; set; }
        public ICollection<TestBooking> TestBookings { get; set; }
        public ICollection<AvailableSlot> AvailableSlotsBooked { get; set; }
        public ICollection<ReproductiveCycle> ReproductiveCycles { get; set; }
        public ICollection<CycleNotification> CycleNotifications { get; set; }
        public ICollection<QAThread> QAThreadsAsked { get; set; }
        public ICollection<Blog> Blogs { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}
