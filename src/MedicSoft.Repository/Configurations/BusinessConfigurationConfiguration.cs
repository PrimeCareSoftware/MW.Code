using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class BusinessConfigurationConfiguration : IEntityTypeConfiguration<BusinessConfiguration>
    {
        public void Configure(EntityTypeBuilder<BusinessConfiguration> builder)
        {
            builder.ToTable("BusinessConfigurations");
            
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedNever();
                
            builder.Property(x => x.ClinicId)
                .IsRequired();
                
            builder.Property(x => x.BusinessType)
                .IsRequired()
                .HasConversion<int>();
                
            builder.Property(x => x.PrimarySpecialty)
                .IsRequired()
                .HasConversion<int>();
                
            builder.Property(x => x.TenantId)
                .IsRequired()
                .HasMaxLength(100);
                
            // Feature flags - all boolean properties
            builder.Property(x => x.ElectronicPrescription).IsRequired();
            builder.Property(x => x.LabIntegration).IsRequired();
            builder.Property(x => x.VaccineControl).IsRequired();
            builder.Property(x => x.InventoryManagement).IsRequired();
            builder.Property(x => x.MultiRoom).IsRequired();
            builder.Property(x => x.ReceptionQueue).IsRequired();
            builder.Property(x => x.FinancialModule).IsRequired();
            builder.Property(x => x.HealthInsurance).IsRequired();
            builder.Property(x => x.Telemedicine).IsRequired();
            builder.Property(x => x.HomeVisit).IsRequired();
            builder.Property(x => x.GroupSessions).IsRequired();
            builder.Property(x => x.PublicProfile).IsRequired();
            builder.Property(x => x.OnlineBooking).IsRequired();
            builder.Property(x => x.PatientReviews).IsRequired();
            builder.Property(x => x.BiReports).IsRequired();
            builder.Property(x => x.ApiAccess).IsRequired();
            builder.Property(x => x.WhiteLabel).IsRequired();
            
            builder.Property(x => x.CreatedAt)
                .IsRequired();
                
            builder.Property(x => x.UpdatedAt)
                .IsRequired();
            
            // Indexes
            builder.HasIndex(x => new { x.TenantId, x.ClinicId })
                .IsUnique()
                .HasDatabaseName("IX_BusinessConfigurations_TenantId_ClinicId");
                
            builder.HasIndex(x => x.TenantId)
                .HasDatabaseName("IX_BusinessConfigurations_TenantId");
            
            // Relationships
            builder.HasOne(x => x.Clinic)
                .WithMany()
                .HasForeignKey(x => x.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
