using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class WaitingQueueConfigurationConfiguration : IEntityTypeConfiguration<WaitingQueueConfiguration>
    {
        public void Configure(EntityTypeBuilder<WaitingQueueConfiguration> builder)
        {
            builder.ToTable("WaitingQueueConfigurations");

            builder.HasKey(wqc => wqc.Id);

            builder.Property(wqc => wqc.Id)
                .ValueGeneratedNever();

            builder.Property(wqc => wqc.DisplayMode)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(wqc => wqc.ShowEstimatedWaitTime)
                .IsRequired();

            builder.Property(wqc => wqc.ShowPatientNames)
                .IsRequired();

            builder.Property(wqc => wqc.ShowPriority)
                .IsRequired();

            builder.Property(wqc => wqc.ShowPosition)
                .IsRequired();

            builder.Property(wqc => wqc.AutoRefreshSeconds)
                .IsRequired();

            builder.Property(wqc => wqc.EnableSoundNotifications)
                .IsRequired();

            builder.Property(wqc => wqc.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(wqc => wqc.Clinic)
                .WithMany()
                .HasForeignKey(wqc => wqc.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(wqc => new { wqc.TenantId, wqc.ClinicId })
                .HasDatabaseName("IX_WaitingQueueConfigurations_TenantId_Clinic")
                .IsUnique();

            builder.HasIndex(wqc => wqc.TenantId)
                .HasDatabaseName("IX_WaitingQueueConfigurations_TenantId");
        }
    }
}
