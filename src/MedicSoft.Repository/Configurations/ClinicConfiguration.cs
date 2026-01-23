using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {
            builder.ToTable("Clinics");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedNever();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.TradeName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Document)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.DocumentType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);

            builder.Property(c => c.Phone)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(c => c.Address)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ShowOnPublicSite)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.ClinicType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(c => c.WhatsAppNumber)
                .HasMaxLength(30);

            builder.Property(c => c.DefaultPaymentReceiverType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(c => c.NotifyPrimaryDoctorOnOtherDoctorAppointment)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationships
            builder.HasOne(c => c.Company)
                .WithMany()
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); // Optional for backward compatibility

            // Indexes
            builder.HasIndex(c => new { c.TenantId, c.Document })
                .IsUnique()
                .HasDatabaseName("IX_Clinics_TenantId_Document");

            builder.HasIndex(c => c.TenantId)
                .HasDatabaseName("IX_Clinics_TenantId");

            builder.HasIndex(c => c.ShowOnPublicSite)
                .HasDatabaseName("IX_Clinics_ShowOnPublicSite");

            builder.HasIndex(c => c.CompanyId)
                .HasDatabaseName("IX_Clinics_CompanyId");
        }
    }
}