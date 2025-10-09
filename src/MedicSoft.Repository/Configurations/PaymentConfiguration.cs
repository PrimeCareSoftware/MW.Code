using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.Method)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.PaymentDate)
                .IsRequired();

            builder.Property(p => p.ProcessedDate)
                .IsRequired(false);

            builder.Property(p => p.TransactionId)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(p => p.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(p => p.CancellationReason)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(p => p.CancellationDate)
                .IsRequired(false);

            builder.Property(p => p.CardLastFourDigits)
                .HasMaxLength(4)
                .IsRequired(false);

            builder.Property(p => p.PixKey)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(p => p.PixTransactionId)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(p => p.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false);

            // Relationships
            builder.HasOne(p => p.Appointment)
                .WithMany()
                .HasForeignKey(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(p => p.ClinicSubscription)
                .WithMany()
                .HasForeignKey(p => p.ClinicSubscriptionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(p => p.Invoice)
                .WithOne(i => i.Payment)
                .HasForeignKey<Invoice>(i => i.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(p => p.AppointmentId);
            builder.HasIndex(p => p.ClinicSubscriptionId);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.PaymentDate);
            builder.HasIndex(p => p.TenantId);
        }
    }
}
