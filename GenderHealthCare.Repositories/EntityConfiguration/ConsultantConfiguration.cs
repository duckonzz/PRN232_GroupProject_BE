using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class ConsultantConfiguration : IEntityTypeConfiguration<Consultant>
    {
        public void Configure(EntityTypeBuilder<Consultant> builder)
        {
            builder.ToTable("Consultants");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Degree)
                   .HasMaxLength(100);

            builder.Property(x => x.ExperienceYears)
                   .IsRequired();

            builder.Property(x => x.Bio)
                   .HasMaxLength(1000);

            builder.Property(x => x.UserId)
                   .IsRequired();

            // Relationships

            builder.HasOne(x => x.User)
                   .WithOne(u => u.ConsultantProfile)
                   .HasForeignKey<Consultant>(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Schedules)
                   .WithOne(s => s.Consultant)
                   .HasForeignKey(s => s.ConsultantId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Consultations)
                   .WithOne(c => c.Consultant)
                   .HasForeignKey(c => c.ConsultantId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.QAThreads)
                   .WithOne(q => q.Consultant)
                   .HasForeignKey(q => q.ConsultantId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
