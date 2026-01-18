using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class AccountsPayableConfiguration : IEntityTypeConfiguration<AccountsPayable>
    {
        public void Configure(EntityTypeBuilder<AccountsPayable> builder)
        {
            builder.ToTable("AccountsPayable");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.DocumentNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Category)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.PaidAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.OutstandingAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(p => p.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(p => p.CancellationReason)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(p => p.BankName)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(p => p.BankAccount)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(p => p.PixKey)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(p => p.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Relationships
            builder.HasOne(p => p.Supplier)
                .WithMany()
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasMany(p => p.Payments)
                .WithOne(pp => pp.Payable)
                .HasForeignKey(pp => pp.PayableId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => p.DocumentNumber);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.Category);
            builder.HasIndex(p => p.DueDate);
            builder.HasIndex(p => p.SupplierId);
            builder.HasIndex(p => p.TenantId);
        }
    }

    public class PayablePaymentConfiguration : IEntityTypeConfiguration<PayablePayment>
    {
        public void Configure(EntityTypeBuilder<PayablePayment> builder)
        {
            builder.ToTable("PayablePayments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.TransactionId)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(p => p.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(p => p.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Indexes
            builder.HasIndex(p => p.PayableId);
            builder.HasIndex(p => p.PaymentDate);
            builder.HasIndex(p => p.TenantId);
        }
    }
}
