using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;

namespace GenderHealthCare.Repositories.Base
{
    public class GenderHealthCareDbContext : DbContext
    {
        public GenderHealthCareDbContext(DbContextOptions<GenderHealthCareDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<ConsultantSchedule> ConsultantSchedules { get; set; }
        public DbSet<AvailableSlot> AvailableSlots { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<HealthTest> HealthTests { get; set; }
        public DbSet<HealthTestSchedule> HealthTestSchedules { get; set; }
        public DbSet<TestSlot> TestSlots { get; set; }
        public DbSet<TestBooking> TestBookings { get; set; }
        public DbSet<ReproductiveCycle> ReproductiveCycles { get; set; }
        public DbSet<CycleNotification> CycleNotifications { get; set; }
        public DbSet<QAThread> QAThreads { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GenderHealthCareDbContext).Assembly);
        }
    }
}