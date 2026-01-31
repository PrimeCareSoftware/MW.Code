using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;
using System;
using System.Linq;
using System.Text.Json;

namespace MedicSoft.Repository.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedNever();

            builder.Property(a => a.Timestamp)
                .IsRequired();

            builder.Property(a => a.UserId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.UserName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.UserEmail)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.ActionDescription)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.EntityId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.EntityDisplayName)
                .HasMaxLength(500);

            builder.Property(a => a.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.UserAgent)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.RequestPath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.HttpMethod)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(a => a.OldValues)
                .HasColumnType("text");

            builder.Property(a => a.NewValues)
                .HasColumnType("text");

            builder.Property(a => a.ChangedFields)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
                    v => v != null ? JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) : null
                )
                .HasColumnType("text")
                .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>?>(
                    (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                    c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c == null ? new List<string>() : c.ToList()));

            builder.Property(a => a.Result)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.FailureReason)
                .HasMaxLength(1000);

            builder.Property(a => a.StatusCode);

            builder.Property(a => a.DataCategory)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.Purpose)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.Severity)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Composite indexes for high-performance queries
            builder.HasIndex(a => new { a.TenantId, a.UserId, a.Timestamp })
                .HasDatabaseName("IX_AuditLogs_Tenant_User_Time");

            builder.HasIndex(a => new { a.TenantId, a.EntityType, a.EntityId })
                .HasDatabaseName("IX_AuditLogs_Tenant_Entity");

            builder.HasIndex(a => new { a.TenantId, a.Action, a.Timestamp })
                .HasDatabaseName("IX_AuditLogs_Tenant_Action_Time");

            builder.HasIndex(a => new { a.TenantId, a.Timestamp })
                .HasDatabaseName("IX_AuditLogs_Tenant_Time");

            builder.HasIndex(a => new { a.TenantId, a.Severity })
                .HasDatabaseName("IX_AuditLogs_Tenant_Severity");

            // Partial index for high-severity events (better performance for security queries)
            builder.HasIndex(a => new { a.TenantId, a.Severity, a.Timestamp })
                .HasDatabaseName("IX_AuditLogs_Tenant_HighSeverity_Time")
                .HasFilter("\"Severity\" IN ('WARNING', 'ERROR', 'CRITICAL')");

            // Single column indexes for specific queries
            builder.HasIndex(a => a.UserId)
                .HasDatabaseName("IX_AuditLogs_UserId");

            builder.HasIndex(a => a.Timestamp)
                .HasDatabaseName("IX_AuditLogs_Timestamp");
        }
    }

    public class DataProcessingConsentConfiguration : IEntityTypeConfiguration<DataProcessingConsent>
    {
        public void Configure(EntityTypeBuilder<DataProcessingConsent> builder)
        {
            builder.ToTable("DataProcessingConsents");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedNever();

            builder.Property(c => c.UserId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ConsentDate)
                .IsRequired();

            builder.Property(c => c.RevokedDate);

            builder.Property(c => c.IsRevoked)
                .IsRequired();

            builder.Property(c => c.Purpose)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(c => c.PurposeDescription)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.DataCategories)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Domain.Enums.DataCategory>>(v, (JsonSerializerOptions?)null) ?? new List<Domain.Enums.DataCategory>()
                )
                .HasColumnType("text")
                .IsRequired()
                .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<Domain.Enums.DataCategory>>(
                    (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                    c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c == null ? new List<Domain.Enums.DataCategory>() : c.ToList()));

            builder.Property(c => c.ConsentText)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(c => c.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.UserAgent)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.ConsentMethod)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(c => c.UserId);
            builder.HasIndex(c => c.TenantId);
            builder.HasIndex(c => c.ConsentDate);
        }
    }
}
