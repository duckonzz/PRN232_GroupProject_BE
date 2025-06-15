using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class ReproductiveCycleConfiguration : IEntityTypeConfiguration<ReproductiveCycle>
    {
        public void Configure(EntityTypeBuilder<ReproductiveCycle> builder)
        {
            builder.ToTable("ReproductiveCycles");

            builder.Property(x => x.StartDate)
                   .IsRequired()
                   .HasColumnType("date");

            builder.Property(x => x.EndDate)
                   .IsRequired()
                   .HasColumnType("date");

            builder.Property(x => x.CycleLength)
                   .IsRequired();

            builder.Property(x => x.PeriodLength)
                   .IsRequired();

            builder.Property(x => x.Notes)
                   .HasMaxLength(1000);

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.HasOne(x => x.User)
                   .WithMany(u => u.ReproductiveCycles)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Notifications)
                   .WithOne(n => n.ReproductiveCycle)
                   .HasForeignKey(n => n.ReproductiveCycleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
