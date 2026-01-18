using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class CashFlowEntryConfiguration : IEntityTypeConfiguration<CashFlowEntry>
    {
        public void Configure(EntityTypeBuilder<CashFlowEntry> builder)
        {
            builder.ToTable("CashFlowEntries");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Type)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(e => e.Category)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.Reference)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(e => e.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(e => e.BankAccount)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(e => e.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Relationships
            builder.HasOne(e => e.Payment)
                .WithMany()
                .HasForeignKey(e => e.PaymentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(e => e.Receivable)
                .WithMany()
                .HasForeignKey(e => e.ReceivableId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(e => e.Payable)
                .WithMany()
                .HasForeignKey(e => e.PayableId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(e => e.Appointment)
                .WithMany()
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(e => e.Type);
            builder.HasIndex(e => e.Category);
            builder.HasIndex(e => e.TransactionDate);
            builder.HasIndex(e => e.AppointmentId);
            builder.HasIndex(e => e.TenantId);
        }
    }
}
