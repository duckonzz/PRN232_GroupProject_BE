using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class AvailableSlotConfiguration : IEntityTypeConfiguration<AvailableSlot>
    {
        public void Configure(EntityTypeBuilder<AvailableSlot> builder)
        {
            builder.ToTable("AvailableSlots");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SlotStart)
                   .IsRequired()
                   .HasColumnType("time");

            builder.Property(x => x.SlotEnd)
                   .IsRequired()
                   .HasColumnType("time");

            builder.Property(x => x.IsBooked)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(x => x.BookedAt)
                   .HasColumnType("datetimeoffset");

            builder.Property(x => x.ScheduleId)
                   .IsRequired();

            builder.HasOne(x => x.Schedule)
                   .WithMany(s => s.Slots)
                   .HasForeignKey(x => x.ScheduleId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BookedByUser)
                   .WithMany(u => u.AvailableSlotsBooked)
                   .HasForeignKey(x => x.BookedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(x => x.Consultations)
                   .WithOne(c => c.Slot)
                   .HasForeignKey(c => c.SlotId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
