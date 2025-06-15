using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class CycleNotificationConfiguration : IEntityTypeConfiguration<CycleNotification>
    {
        public void Configure(EntityTypeBuilder<CycleNotification> builder)
        {
            builder.ToTable("CycleNotifications");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NotificationType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.NotificationDate)
                   .IsRequired()
                   .HasColumnType("date");

            builder.Property(x => x.IsSent)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(x => x.SentAt)
                   .HasColumnType("datetimeoffset");

            builder.Property(x => x.Message)
                   .HasMaxLength(500);

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.HasOne(x => x.User)
                   .WithMany(u => u.CycleNotifications)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ReproductiveCycleId)
                   .IsRequired();

            builder.HasOne(x => x.ReproductiveCycle)
                   .WithMany(rc => rc.Notifications)
                   .HasForeignKey(x => x.ReproductiveCycleId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
