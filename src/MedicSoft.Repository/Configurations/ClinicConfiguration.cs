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

            // Indexes
            builder.HasIndex(c => new { c.TenantId, c.Document })
                .IsUnique()
                .HasDatabaseName("IX_Clinics_TenantId_Document");

            builder.HasIndex(c => c.TenantId)
                .HasDatabaseName("IX_Clinics_TenantId");
        }
    }
}