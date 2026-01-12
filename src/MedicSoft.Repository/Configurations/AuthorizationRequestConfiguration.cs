using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class AuthorizationRequestConfiguration : IEntityTypeConfiguration<AuthorizationRequest>
    {
        public void Configure(EntityTypeBuilder<AuthorizationRequest> builder)
        {
            builder.ToTable("AuthorizationRequests");

            builder.HasKey(ar => ar.Id);

            builder.Property(ar => ar.Id)
                .ValueGeneratedNever();

            builder.Property(ar => ar.RequestNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ar => ar.AuthorizationNumber)
                .HasMaxLength(50);

            builder.Property(ar => ar.DenialReason)
                .HasMaxLength(1000);

            builder.Property(ar => ar.ProcedureCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(ar => ar.ProcedureDescription)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(ar => ar.ClinicalIndication)
                .HasMaxLength(2000);

            builder.Property(ar => ar.Diagnosis)
                .HasMaxLength(200);

            builder.Property(ar => ar.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(ar => ar.Patient)
                .WithMany()
                .HasForeignKey(ar => ar.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ar => ar.PatientHealthInsurance)
                .WithMany()
                .HasForeignKey(ar => ar.PatientHealthInsuranceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ar => ar.Appointment)
                .WithMany()
                .HasForeignKey(ar => ar.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(ar => new { ar.TenantId, ar.RequestNumber })
                .IsUnique()
                .HasDatabaseName("IX_AuthorizationRequests_TenantId_RequestNumber");

            builder.HasIndex(ar => new { ar.TenantId, ar.Status })
                .HasDatabaseName("IX_AuthorizationRequests_TenantId_Status");

            builder.HasIndex(ar => ar.TenantId)
                .HasDatabaseName("IX_AuthorizationRequests_TenantId");

            builder.HasIndex(ar => ar.AuthorizationNumber)
                .HasDatabaseName("IX_AuthorizationRequests_AuthorizationNumber");
        }
    }
}
