using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class BlogContentConfiguration : IEntityTypeConfiguration<BlogContent>
    {
        public void Configure(EntityTypeBuilder<BlogContent> builder)
        {
            builder.ToTable("BlogContents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Detail)
                   .IsRequired();

            builder.Property(x => x.BlogId)
                   .IsRequired();

            builder.HasOne(x => x.Blog)
                   .WithMany(b => b.Contents)
                   .HasForeignKey(x => x.BlogId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
