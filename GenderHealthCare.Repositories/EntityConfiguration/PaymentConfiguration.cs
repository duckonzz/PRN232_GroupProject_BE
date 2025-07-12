using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenderHealthCare.Repositories.EntityConfiguration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.UserId).IsRequired();
            builder.Property(p => p.ServiceId).IsRequired();
            builder.Property(p => p.Amount).IsRequired();
            builder.Property(p => p.TxnRef).IsRequired();
            builder.Property(p => p.OrderInfo).HasMaxLength(255);
            builder.Property(p => p.BankCode).HasMaxLength(20);
            builder.Property(p => p.ResponseCode).HasMaxLength(10);
            builder.Property(p => p.TransactionStatus).HasMaxLength(10);
            builder.Property(p => p.SecureHash).HasMaxLength(512);
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(p => p.IsSuccess).HasDefaultValue(false);
        }
    }
}
