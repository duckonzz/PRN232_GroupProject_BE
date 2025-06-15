using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
    {
        public void Configure(EntityTypeBuilder<Consultation> builder)
        {
            builder.ToTable("Consultations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Reason)
                   .HasMaxLength(1000);

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasDefaultValue("Pending");

            builder.Property(x => x.Result)
                   .HasMaxLength(2000);

            builder.Property(x => x.SlotId)
                   .IsRequired();

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.ConsultantId)
                   .IsRequired();

            builder.HasOne(x => x.Slot)
                   .WithMany(s => s.Consultations)
                   .HasForeignKey(x => x.SlotId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User)
                   .WithMany(u => u.Consultations)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Consultant)
                   .WithMany(c => c.Consultations)
                   .HasForeignKey(x => x.ConsultantId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
