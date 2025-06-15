using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReportType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.PeriodStart)
                   .IsRequired()
                   .HasColumnType("date");

            builder.Property(x => x.PeriodEnd)
                   .IsRequired()
                   .HasColumnType("date");

            builder.Property(x => x.Content)
                   .IsRequired();

            builder.Property(x => x.Notes)
                   .HasMaxLength(500);

            builder.Property(x => x.GeneratedByUserId)
                   .IsRequired();

            builder.HasOne(x => x.GeneratedByUser)
                   .WithMany(u => u.Reports)
                   .HasForeignKey(x => x.GeneratedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
