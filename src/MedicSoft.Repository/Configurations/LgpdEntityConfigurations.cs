using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class DataAccessLogConfiguration : IEntityTypeConfiguration<DataAccessLog>
    {
        public void Configure(EntityTypeBuilder<DataAccessLog> builder)
        {
            builder.ToTable("data_access_logs");
            
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Id)
                .HasColumnName("id")
                .IsRequired();

            builder.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(e => e.UserName)
                .HasColumnName("user_name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.UserRole)
                .HasColumnName("user_role")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.EntityType)
                .HasColumnName("entity_type")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.EntityId)
                .HasColumnName("entity_id")
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(e => e.FieldsAccessed)
                .HasColumnName("fields_accessed")
                .HasColumnType("jsonb");

            builder.Property(e => e.PatientId)
                .HasColumnName("patient_id");

            builder.Property(e => e.PatientName)
                .HasColumnName("patient_name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.Timestamp)
                .HasColumnName("timestamp")
                .IsRequired();

            builder.Property(e => e.AccessReason)
                .HasColumnName("access_reason")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.IpAddress)
                .HasColumnName("ip_address")
                .HasMaxLength(45)
                .IsRequired();

            builder.Property(e => e.Location)
                .HasColumnName("location")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.WasAuthorized)
                .HasColumnName("was_authorized")
                .IsRequired();

            builder.Property(e => e.DenialReason)
                .HasColumnName("denial_reason")
                .HasMaxLength(500);

            builder.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            // Indexes
            builder.HasIndex(e => e.PatientId)
                .HasDatabaseName("idx_data_access_logs_patient_id");

            builder.HasIndex(e => e.UserId)
                .HasDatabaseName("idx_data_access_logs_user_id");

            builder.HasIndex(e => e.Timestamp)
                .HasDatabaseName("idx_data_access_logs_timestamp");

            builder.HasIndex(e => new { e.TenantId, e.Timestamp })
                .HasDatabaseName("idx_data_access_logs_tenant_timestamp");

            builder.HasIndex(e => e.WasAuthorized)
                .HasDatabaseName("idx_data_access_logs_was_authorized");
        }
    }

    public class DataConsentLogConfiguration : IEntityTypeConfiguration<DataConsentLog>
    {
        public void Configure(EntityTypeBuilder<DataConsentLog> builder)
        {
            builder.ToTable("data_consent_logs");
            
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Id)
                .HasColumnName("id")
                .IsRequired();

            builder.Property(e => e.PatientId)
                .HasColumnName("patient_id")
                .IsRequired();

            builder.Property(e => e.PatientName)
                .HasColumnName("patient_name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.Purpose)
                .HasColumnName("purpose")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.ConsentDate)
                .HasColumnName("consent_date")
                .IsRequired();

            builder.Property(e => e.ExpirationDate)
                .HasColumnName("expiration_date");

            builder.Property(e => e.RevokedDate)
                .HasColumnName("revoked_date");

            builder.Property(e => e.RevocationReason)
                .HasColumnName("revocation_reason")
                .HasMaxLength(500);

            builder.Property(e => e.IpAddress)
                .HasColumnName("ip_address")
                .HasMaxLength(45)
                .IsRequired();

            builder.Property(e => e.ConsentText)
                .HasColumnName("consent_text")
                .HasColumnType("text")
                .IsRequired();

            builder.Property(e => e.ConsentVersion)
                .HasColumnName("consent_version")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.ConsentMethod)
                .HasColumnName("consent_method")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.UserAgent)
                .HasColumnName("user_agent")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            // Indexes
            builder.HasIndex(e => e.PatientId)
                .HasDatabaseName("idx_data_consent_logs_patient_id");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("idx_data_consent_logs_status");

            builder.HasIndex(e => new { e.PatientId, e.Status })
                .HasDatabaseName("idx_data_consent_logs_patient_status");

            builder.HasIndex(e => e.ConsentDate)
                .HasDatabaseName("idx_data_consent_logs_consent_date");
        }
    }

    public class DataDeletionRequestConfiguration : IEntityTypeConfiguration<DataDeletionRequest>
    {
        public void Configure(EntityTypeBuilder<DataDeletionRequest> builder)
        {
            builder.ToTable("data_deletion_requests");
            
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Id)
                .HasColumnName("id")
                .IsRequired();

            builder.Property(e => e.PatientId)
                .HasColumnName("patient_id")
                .IsRequired();

            builder.Property(e => e.PatientName)
                .HasColumnName("patient_name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.PatientEmail)
                .HasColumnName("patient_email")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.RequestDate)
                .HasColumnName("request_date")
                .IsRequired();

            builder.Property(e => e.Reason)
                .HasColumnName("reason")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(e => e.RequestType)
                .HasColumnName("request_type")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.ProcessedDate)
                .HasColumnName("processed_date");

            builder.Property(e => e.CompletedDate)
                .HasColumnName("completed_date");

            builder.Property(e => e.ProcessedByUserId)
                .HasColumnName("processed_by_user_id")
                .HasMaxLength(450);

            builder.Property(e => e.ProcessedByUserName)
                .HasColumnName("processed_by_user_name")
                .HasMaxLength(200);

            builder.Property(e => e.ProcessingNotes)
                .HasColumnName("processing_notes")
                .HasMaxLength(2000);

            builder.Property(e => e.RejectionReason)
                .HasColumnName("rejection_reason")
                .HasMaxLength(1000);

            builder.Property(e => e.IpAddress)
                .HasColumnName("ip_address")
                .HasMaxLength(45)
                .IsRequired();

            builder.Property(e => e.UserAgent)
                .HasColumnName("user_agent")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.RequiresLegalApproval)
                .HasColumnName("requires_legal_approval")
                .IsRequired();

            builder.Property(e => e.LegalApprovalDate)
                .HasColumnName("legal_approval_date");

            builder.Property(e => e.LegalApprover)
                .HasColumnName("legal_approver")
                .HasMaxLength(200);

            builder.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            // Indexes
            builder.HasIndex(e => e.PatientId)
                .HasDatabaseName("idx_data_deletion_requests_patient_id");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("idx_data_deletion_requests_status");

            builder.HasIndex(e => new { e.TenantId, e.Status })
                .HasDatabaseName("idx_data_deletion_requests_tenant_status");

            builder.HasIndex(e => e.RequestDate)
                .HasDatabaseName("idx_data_deletion_requests_request_date");
        }
    }
}
