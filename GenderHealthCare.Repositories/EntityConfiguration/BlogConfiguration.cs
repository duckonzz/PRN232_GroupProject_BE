using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("Blogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.AuthorId)
                   .IsRequired();

            builder.Property(x => x.Headline)
                   .HasMaxLength(300);

            builder.Property(x => x.Summary)
                   .HasMaxLength(1000);

            builder.Property(x => x.ThumbnailUrl)
                   .HasMaxLength(500);

            builder.HasOne(x => x.Author)
                   .WithMany(u => u.Blogs)
                   .HasForeignKey(x => x.AuthorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Contents)
                   .WithOne(c => c.Blog)
                   .HasForeignKey(c => c.BlogId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
