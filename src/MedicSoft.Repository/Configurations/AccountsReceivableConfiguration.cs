using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class AccountsReceivableConfiguration : IEntityTypeConfiguration<AccountsReceivable>
    {
        public void Configure(EntityTypeBuilder<AccountsReceivable> builder)
        {
            builder.ToTable("AccountsReceivable");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.DocumentNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(r => r.Type)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(r => r.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(r => r.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(r => r.PaidAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(r => r.OutstandingAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(r => r.InterestRate)
                .HasColumnType("decimal(5,2)")
                .IsRequired(false);

            builder.Property(r => r.FineRate)
                .HasColumnType("decimal(5,2)")
                .IsRequired(false);

            builder.Property(r => r.DiscountRate)
                .HasColumnType("decimal(5,2)")
                .IsRequired(false);

            builder.Property(r => r.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(r => r.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(r => r.CancellationReason)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(r => r.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Relationships
            builder.HasOne(r => r.Appointment)
                .WithMany()
                .HasForeignKey(r => r.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(r => r.Patient)
                .WithMany()
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(r => r.HealthInsuranceOperator)
                .WithMany()
                .HasForeignKey(r => r.HealthInsuranceOperatorId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasMany(r => r.Payments)
                .WithOne(p => p.Receivable)
                .HasForeignKey(p => p.ReceivableId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(r => r.DocumentNumber);
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.DueDate);
            builder.HasIndex(r => r.AppointmentId);
            builder.HasIndex(r => r.PatientId);
            builder.HasIndex(r => r.TenantId);
        }
    }

    public class ReceivablePaymentConfiguration : IEntityTypeConfiguration<ReceivablePayment>
    {
        public void Configure(EntityTypeBuilder<ReceivablePayment> builder)
        {
            builder.ToTable("ReceivablePayments");

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
            builder.HasIndex(p => p.ReceivableId);
            builder.HasIndex(p => p.PaymentDate);
            builder.HasIndex(p => p.TenantId);
        }
    }
}
