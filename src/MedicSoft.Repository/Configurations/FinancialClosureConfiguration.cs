using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class FinancialClosureConfiguration : IEntityTypeConfiguration<FinancialClosure>
    {
        public void Configure(EntityTypeBuilder<FinancialClosure> builder)
        {
            builder.ToTable("FinancialClosures");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.ClosureNumber)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.PaymentType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(c => c.PatientAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(c => c.InsuranceAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(c => c.PaidAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(c => c.OutstandingAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(c => c.DiscountAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired(false);

            builder.Property(c => c.DiscountReason)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(c => c.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(c => c.CancellationReason)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(c => c.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Relationships
            builder.HasOne(c => c.Appointment)
                .WithMany()
                .HasForeignKey(c => c.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(c => c.Patient)
                .WithMany()
                .HasForeignKey(c => c.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(c => c.HealthInsuranceOperator)
                .WithMany()
                .HasForeignKey(c => c.HealthInsuranceOperatorId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasMany(c => c.Items)
                .WithOne(i => i.Closure)
                .HasForeignKey(i => i.ClosureId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(c => c.ClosureNumber).IsUnique();
            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => c.ClosureDate);
            builder.HasIndex(c => c.AppointmentId);
            builder.HasIndex(c => c.PatientId);
            builder.HasIndex(c => c.TenantId);
        }
    }

    public class FinancialClosureItemConfiguration : IEntityTypeConfiguration<FinancialClosureItem>
    {
        public void Configure(EntityTypeBuilder<FinancialClosureItem> builder)
        {
            builder.ToTable("FinancialClosureItems");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Description)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(i => i.Quantity)
                .HasColumnType("decimal(18,3)")
                .IsRequired();

            builder.Property(i => i.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.TotalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Indexes
            builder.HasIndex(i => i.ClosureId);
            builder.HasIndex(i => i.TenantId);
        }
    }
}
