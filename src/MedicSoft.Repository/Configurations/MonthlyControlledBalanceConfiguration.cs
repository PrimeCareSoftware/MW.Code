using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class MonthlyControlledBalanceConfiguration : IEntityTypeConfiguration<MonthlyControlledBalance>
    {
        public void Configure(EntityTypeBuilder<MonthlyControlledBalance> builder)
        {
            builder.ToTable("MonthlyControlledBalances");

            builder.HasKey(mcb => mcb.Id);

            builder.Property(mcb => mcb.Id)
                .ValueGeneratedNever();

            builder.Property(mcb => mcb.Year)
                .IsRequired();

            builder.Property(mcb => mcb.Month)
                .IsRequired();

            builder.Property(mcb => mcb.MedicationName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(mcb => mcb.ActiveIngredient)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(mcb => mcb.AnvisaList)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(mcb => mcb.InitialBalance)
                .IsRequired()
                .HasPrecision(18, 3);

            builder.Property(mcb => mcb.TotalIn)
                .IsRequired()
                .HasPrecision(18, 3);

            builder.Property(mcb => mcb.TotalOut)
                .IsRequired()
                .HasPrecision(18, 3);

            builder.Property(mcb => mcb.CalculatedFinalBalance)
                .IsRequired()
                .HasPrecision(18, 3);

            builder.Property(mcb => mcb.PhysicalBalance)
                .HasPrecision(18, 3);

            builder.Property(mcb => mcb.Discrepancy)
                .HasPrecision(18, 3);

            builder.Property(mcb => mcb.DiscrepancyReason)
                .HasMaxLength(500);

            builder.Property(mcb => mcb.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(mcb => mcb.ClosedAt);

            builder.Property(mcb => mcb.ClosedByUserId);

            builder.Property(mcb => mcb.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(mcb => mcb.CreatedAt)
                .IsRequired();

            builder.Property(mcb => mcb.UpdatedAt);

            // Relationships
            builder.HasOne(mcb => mcb.ClosedBy)
                .WithMany()
                .HasForeignKey(mcb => mcb.ClosedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(mcb => new { mcb.TenantId, mcb.Year, mcb.Month, mcb.MedicationName })
                .IsUnique()
                .HasDatabaseName("IX_MonthlyControlledBalances_TenantId_Year_Month_Medication");

            builder.HasIndex(mcb => new { mcb.TenantId, mcb.Status })
                .HasDatabaseName("IX_MonthlyControlledBalances_TenantId_Status");

            builder.HasIndex(mcb => new { mcb.TenantId, mcb.Year, mcb.Month })
                .HasDatabaseName("IX_MonthlyControlledBalances_TenantId_Year_Month");

            builder.HasIndex(mcb => new { mcb.TenantId, mcb.MedicationName, mcb.Year })
                .HasDatabaseName("IX_MonthlyControlledBalances_TenantId_Medication_Year");
        }
    }
}
