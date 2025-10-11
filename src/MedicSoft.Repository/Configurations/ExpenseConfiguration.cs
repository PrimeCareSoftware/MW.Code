using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expenses");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Description)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.Category)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(e => e.DueDate)
                .IsRequired();

            builder.Property(e => e.PaidDate)
                .IsRequired(false);

            builder.Property(e => e.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(e => e.PaymentMethod)
                .HasConversion<int>()
                .IsRequired(false);

            builder.Property(e => e.PaymentReference)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(e => e.SupplierName)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(e => e.SupplierDocument)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(e => e.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(e => e.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .IsRequired(false);

            // Relationships
            builder.HasOne(e => e.Clinic)
                .WithMany()
                .HasForeignKey(e => e.ClinicId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Indexes
            builder.HasIndex(e => e.ClinicId);
            builder.HasIndex(e => e.DueDate);
            builder.HasIndex(e => e.Status);
            builder.HasIndex(e => e.Category);
            builder.HasIndex(e => e.TenantId);
        }
    }
}
