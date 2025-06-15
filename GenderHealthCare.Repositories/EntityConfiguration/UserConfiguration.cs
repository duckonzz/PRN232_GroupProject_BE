using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.HasIndex(x => x.Email)
                   .IsUnique();

            builder.Property(x => x.FullName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.PhoneNumber)
                   .HasMaxLength(20);

            builder.Property(x => x.DateOfBirth)
                   .HasColumnType("date");

            builder.Property(x => x.Gender)
                   .HasMaxLength(20);

            builder.Property(x => x.Role)
                   .IsRequired()
                   .HasMaxLength(20)
                   .HasComment("Customer / Consultant / Staff / Manager / Admin");

            builder.Property(x => x.IsCycleTrackingOn)
                   .HasDefaultValue(false);

            // Relationships

            builder.HasOne(u => u.ConsultantProfile)
                   .WithOne(c => c.User)
                   .HasForeignKey<Consultant>(c => c.UserId);

            builder.HasMany(u => u.Consultations)
                   .WithOne(c => c.User)
                   .HasForeignKey(c => c.UserId);

            builder.HasMany(u => u.Feedbacks)
                   .WithOne(f => f.User)
                   .HasForeignKey(f => f.UserId);

            builder.HasMany(u => u.TestBookings)
                   .WithOne(tb => tb.Customer)
                   .HasForeignKey(tb => tb.CustomerId);

            builder.HasMany(u => u.ReproductiveCycles)
                   .WithOne(rc => rc.User)
                   .HasForeignKey(rc => rc.UserId);

            builder.HasMany(u => u.CycleNotifications)
                   .WithOne(cn => cn.User)
                   .HasForeignKey(cn => cn.UserId);

            builder.HasMany(u => u.QAThreadsAsked)
                   .WithOne(qa => qa.Customer)
                   .HasForeignKey(qa => qa.CustomerId);

            builder.HasMany(u => u.Blogs)
                   .WithOne(b => b.Author)
                   .HasForeignKey(b => b.AuthorId);

            builder.HasMany(u => u.Reports)
                   .WithOne(r => r.GeneratedByUser)
                   .HasForeignKey(r => r.GeneratedByUserId);
        }
    }
}
