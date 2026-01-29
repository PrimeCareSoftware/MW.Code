using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ModuleConfigurationHistoryConfiguration : IEntityTypeConfiguration<ModuleConfigurationHistory>
    {
        public void Configure(EntityTypeBuilder<ModuleConfigurationHistory> builder)
        {
            builder.ToTable("ModuleConfigurationHistories");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.ModuleName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.Action)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(h => h.ChangedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.PreviousConfiguration)
                .HasColumnType("jsonb");

            builder.Property(h => h.NewConfiguration)
                .HasColumnType("jsonb");

            builder.Property(h => h.Reason)
                .HasMaxLength(500);

            builder.HasIndex(h => new { h.ClinicId, h.ModuleName })
                .HasDatabaseName("IX_ModuleConfigurationHistories_ClinicId_ModuleName");

            builder.HasIndex(h => h.ChangedAt)
                .HasDatabaseName("IX_ModuleConfigurationHistories_ChangedAt");

            // Relationship with ModuleConfiguration
            builder.HasOne(h => h.ModuleConfiguration)
                .WithMany()
                .HasForeignKey(h => h.ModuleConfigurationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
