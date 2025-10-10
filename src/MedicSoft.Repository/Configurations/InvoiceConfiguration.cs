using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.InvoiceNumber)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(i => i.Type)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(i => i.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(i => i.IssueDate)
                .IsRequired();

            builder.Property(i => i.DueDate)
                .IsRequired();

            builder.Property(i => i.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.TaxAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.Description)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(i => i.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(i => i.SentDate)
                .IsRequired(false);

            builder.Property(i => i.PaidDate)
                .IsRequired(false);

            builder.Property(i => i.CancellationDate)
                .IsRequired(false);

            builder.Property(i => i.CancellationReason)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(i => i.CustomerName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(i => i.CustomerDocument)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(i => i.CustomerAddress)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(i => i.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(i => i.CreatedAt)
                .IsRequired();

            builder.Property(i => i.UpdatedAt)
                .IsRequired(false);

            // Relationships - Payment relationship defined in PaymentConfiguration

            // Indexes
            builder.HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            builder.HasIndex(i => i.PaymentId);
            builder.HasIndex(i => i.Status);
            builder.HasIndex(i => i.DueDate);
            builder.HasIndex(i => i.TenantId);
        }
    }
}
