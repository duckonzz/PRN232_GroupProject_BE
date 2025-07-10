using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class QAThreadConfiguration : IEntityTypeConfiguration<QAThread>
    {
        public void Configure(EntityTypeBuilder<QAThread> builder)
        {
            builder.ToTable("QAThreads");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Question)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.Answer)
                   .HasMaxLength(1000);

            builder.Property(x => x.AnsweredAt)
                   .HasColumnType("datetimeoffset");

            builder.Property(x => x.CustomerId)
                   .IsRequired();

            builder.HasOne(x => x.Customer)
                   .WithMany(u => u.QAThreadsAsked)
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
