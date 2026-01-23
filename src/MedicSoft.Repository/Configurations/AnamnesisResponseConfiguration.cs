using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class AnamnesisResponseConfiguration : IEntityTypeConfiguration<AnamnesisResponse>
    {
        public void Configure(EntityTypeBuilder<AnamnesisResponse> builder)
        {
            builder.ToTable("AnamnesisResponses");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedNever();

            builder.Property(r => r.AppointmentId)
                .IsRequired();

            builder.Property(r => r.PatientId)
                .IsRequired();

            builder.Property(r => r.DoctorId)
                .IsRequired();

            builder.Property(r => r.TemplateId)
                .IsRequired();

            builder.Property(r => r.ResponseDate)
                .IsRequired();

            builder.Property(r => r.AnswersJson)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(r => r.IsComplete)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(r => r.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.Property(r => r.UpdatedAt);

            // Relationships
            builder.HasOne(r => r.Appointment)
                .WithMany()
                .HasForeignKey(r => r.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Patient)
                .WithMany()
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Doctor)
                .WithMany()
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Template)
                .WithMany()
                .HasForeignKey(r => r.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(r => new { r.TenantId, r.AppointmentId })
                .IsUnique()
                .HasDatabaseName("IX_AnamnesisResponses_TenantId_AppointmentId");

            builder.HasIndex(r => new { r.TenantId, r.PatientId })
                .HasDatabaseName("IX_AnamnesisResponses_TenantId_PatientId");

            builder.HasIndex(r => new { r.TenantId, r.DoctorId })
                .HasDatabaseName("IX_AnamnesisResponses_TenantId_DoctorId");

            builder.HasIndex(r => r.TenantId)
                .HasDatabaseName("IX_AnamnesisResponses_TenantId");

            builder.HasIndex(r => r.ResponseDate)
                .HasDatabaseName("IX_AnamnesisResponses_ResponseDate");
        }
    }
}
