using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class AppointmentProcedureConfiguration : IEntityTypeConfiguration<AppointmentProcedure>
    {
        public void Configure(EntityTypeBuilder<AppointmentProcedure> builder)
        {
            builder.ToTable("AppointmentProcedures");

            builder.HasKey(ap => ap.Id);

            builder.Property(ap => ap.Id)
                .ValueGeneratedNever();

            builder.Property(ap => ap.AppointmentId)
                .IsRequired();

            builder.Property(ap => ap.ProcedureId)
                .IsRequired();

            builder.Property(ap => ap.PatientId)
                .IsRequired();

            builder.Property(ap => ap.PriceCharged)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ap => ap.Notes)
                .HasMaxLength(1000);

            builder.Property(ap => ap.PerformedAt)
                .IsRequired();

            builder.Property(ap => ap.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ap => ap.CreatedAt)
                .IsRequired();

            builder.Property(ap => ap.UpdatedAt);

            // Navigation properties
            builder.HasOne(ap => ap.Appointment)
                .WithMany()
                .HasForeignKey(ap => ap.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ap => ap.Procedure)
                .WithMany()
                .HasForeignKey(ap => ap.ProcedureId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ap => ap.Patient)
                .WithMany()
                .HasForeignKey(ap => ap.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(ap => ap.AppointmentId)
                .HasDatabaseName("IX_AppointmentProcedures_AppointmentId");

            builder.HasIndex(ap => ap.PatientId)
                .HasDatabaseName("IX_AppointmentProcedures_PatientId");
        }
    }
}
