using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class TestBookingConfiguration : IEntityTypeConfiguration<TestBooking>
    {
        public void Configure(EntityTypeBuilder<TestBooking> builder)
        {
            builder.ToTable("TestBookings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasMaxLength(20)
                   .HasDefaultValue("Pending");

            builder.Property(x => x.ResultUrl)
                   .HasMaxLength(500);

            builder.Property(x => x.SlotId)
                   .IsRequired();

            builder.Property(x => x.CustomerId)
                   .IsRequired();

            builder.HasOne(x => x.Slot)
                   .WithMany(s => s.TestBookings)
                   .HasForeignKey(x => x.SlotId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Customer)
                   .WithMany(u => u.TestBookings)
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
