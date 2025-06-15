using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class TestSlotConfiguration : IEntityTypeConfiguration<TestSlot>
    {
        public void Configure(EntityTypeBuilder<TestSlot> builder)
        {
            builder.ToTable("TestSlots");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TestDate)
                   .IsRequired()
                   .HasColumnType("date");

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

            builder.Property(x => x.HealthTestId)
                   .IsRequired();

            builder.HasOne(x => x.HealthTest)
                   .WithMany(h => h.Slots)
                   .HasForeignKey(x => x.HealthTestId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BookedByUser)
                   .WithMany(u => u.TestSlotsBooked)
                   .HasForeignKey(x => x.BookedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.TestBookings)
                   .WithOne(tb => tb.Slot)
                   .HasForeignKey(tb => tb.SlotId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
