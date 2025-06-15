using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class ConsultantScheduleConfiguration : IEntityTypeConfiguration<ConsultantSchedule>
    {
        public void Configure(EntityTypeBuilder<ConsultantSchedule> builder)
        {
            builder.ToTable("ConsultantSchedules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AvailableDate)
                   .IsRequired()
                   .HasColumnType("date");

            builder.Property(x => x.StartTime)
                   .IsRequired()
                   .HasColumnType("time");

            builder.Property(x => x.EndTime)
                   .IsRequired()
                   .HasColumnType("time");

            builder.Property(x => x.ConsultantId)
                   .IsRequired();

            builder.HasOne(cs => cs.Consultant)
                   .WithMany(c => c.Schedules)
                   .HasForeignKey(cs => cs.ConsultantId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(cs => cs.Slots)
                   .WithOne(slot => slot.Schedule)
                   .HasForeignKey(slot => slot.ScheduleId);
        }
    }
}
