using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class HealthTestScheduleConfiguration : IEntityTypeConfiguration<HealthTestSchedule>
    {
        public void Configure(EntityTypeBuilder<HealthTestSchedule> builder)
        {
            builder.ToTable("HealthTestSchedules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.StartDate)
                   .IsRequired()
                   .HasColumnType("date");

            builder.Property(x => x.EndDate)
                   .IsRequired()
                   .HasColumnType("date");

            builder.Property(x => x.SlotStart)
                   .IsRequired()
                   .HasColumnType("time");

            builder.Property(x => x.SlotEnd)
                   .IsRequired()
                   .HasColumnType("time");

            builder.Property(x => x.SlotDurationInMinutes)
                   .IsRequired()
                   .HasDefaultValue(60);

            builder.Property(x => x.DaysOfWeek)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.HealthTestId)
                   .IsRequired();

            builder.HasOne(x => x.HealthTest)
                   .WithMany(ht => ht.HealthTestSchedules)
                   .HasForeignKey(x => x.HealthTestId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
