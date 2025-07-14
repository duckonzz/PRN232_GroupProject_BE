using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class HealthTestConfiguration : IEntityTypeConfiguration<HealthTest>
    {
        public void Configure(EntityTypeBuilder<HealthTest> builder)
        {
            builder.ToTable("HealthTests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Description)
                   .HasMaxLength(1000);

            builder.Property(x => x.Price)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");
        }
    }
}
