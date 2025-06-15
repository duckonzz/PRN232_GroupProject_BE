using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedback");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TargetType)
                   .IsRequired()
                   .HasMaxLength(50); // e.g., "Consultation", "HealthTest"

            builder.Property(x => x.TargetId)
                   .IsRequired();

            builder.Property(x => x.Rating)
                   .IsRequired();

            builder.Property(x => x.Comment)
                   .HasMaxLength(1000);

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.HasOne(x => x.User)
                   .WithMany(u => u.Feedbacks)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
