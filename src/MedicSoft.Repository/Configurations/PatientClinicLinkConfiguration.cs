using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class PatientClinicLinkConfiguration : IEntityTypeConfiguration<PatientClinicLink>
    {
        public void Configure(EntityTypeBuilder<PatientClinicLink> builder)
        {
            builder.ToTable("PatientClinicLinks");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .ValueGeneratedNever();

            builder.Property(l => l.LinkedAt)
                .IsRequired();

            builder.Property(l => l.IsActive)
                .IsRequired();

            builder.Property(l => l.PrimaryDoctorId)
                .IsRequired(false);

            builder.Property(l => l.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(l => l.Patient)
                .WithMany(p => p.ClinicLinks)
                .HasForeignKey(l => l.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.Clinic)
                .WithMany()
                .HasForeignKey(l => l.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.PrimaryDoctor)
                .WithMany()
                .HasForeignKey(l => l.PrimaryDoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(l => new { l.PatientId, l.ClinicId, l.TenantId })
                .IsUnique()
                .HasDatabaseName("IX_PatientClinicLinks_Patient_Clinic_Tenant");

            builder.HasIndex(l => new { l.TenantId, l.ClinicId })
                .HasDatabaseName("IX_PatientClinicLinks_TenantId_ClinicId");

            builder.HasIndex(l => l.PatientId)
                .HasDatabaseName("IX_PatientClinicLinks_PatientId");
        }
    }
}
