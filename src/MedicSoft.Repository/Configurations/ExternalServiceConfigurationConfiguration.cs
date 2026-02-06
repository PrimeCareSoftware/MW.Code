using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ExternalServiceConfigurationConfiguration : IEntityTypeConfiguration<ExternalServiceConfiguration>
    {
        public void Configure(EntityTypeBuilder<ExternalServiceConfiguration> builder)
        {
            builder.ToTable("ExternalServiceConfigurations");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedNever();

            builder.Property(e => e.ServiceType)
                .IsRequired();

            builder.Property(e => e.ServiceName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .HasMaxLength(1000);

            builder.Property(e => e.IsActive)
                .IsRequired();

            // Encrypted fields - increased size for encryption overhead
            builder.Property(e => e.ApiKey)
                .HasMaxLength(1000);

            builder.Property(e => e.ApiSecret)
                .HasMaxLength(1000);

            builder.Property(e => e.ClientId)
                .HasMaxLength(500);

            builder.Property(e => e.ClientSecret)
                .HasMaxLength(1000);

            builder.Property(e => e.AccessToken)
                .HasMaxLength(2000);

            builder.Property(e => e.RefreshToken)
                .HasMaxLength(2000);

            builder.Property(e => e.TokenExpiresAt);

            // Service configuration
            builder.Property(e => e.ApiUrl)
                .HasMaxLength(500);

            builder.Property(e => e.WebhookUrl)
                .HasMaxLength(500);

            builder.Property(e => e.AccountId)
                .HasMaxLength(200);

            builder.Property(e => e.ProjectId)
                .HasMaxLength(200);

            builder.Property(e => e.Region)
                .HasMaxLength(100);

            // Additional configuration (JSON)
            builder.Property(e => e.AdditionalConfiguration);

            // Metadata
            builder.Property(e => e.LastSyncAt);

            builder.Property(e => e.LastError)
                .HasMaxLength(2000);

            builder.Property(e => e.ErrorCount)
                .IsRequired();

            builder.Property(e => e.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.ClinicId);

            // Indexes
            builder.HasIndex(e => e.TenantId)
                .HasDatabaseName("IX_ExternalServiceConfigurations_TenantId");

            builder.HasIndex(e => new { e.TenantId, e.ServiceType, e.ClinicId })
                .IsUnique()
                .HasDatabaseName("IX_ExternalServiceConfigurations_TenantId_ServiceType_ClinicId");

            builder.HasIndex(e => new { e.TenantId, e.IsActive })
                .HasDatabaseName("IX_ExternalServiceConfigurations_TenantId_IsActive");

            builder.HasIndex(e => e.ClinicId)
                .HasDatabaseName("IX_ExternalServiceConfigurations_ClinicId");

            // Relationship with Clinic
            builder.HasOne(e => e.Clinic)
                .WithMany()
                .HasForeignKey(e => e.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
