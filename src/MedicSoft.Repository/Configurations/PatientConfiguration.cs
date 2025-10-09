using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Document)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Gender)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.MedicalHistory)
                .HasMaxLength(2000);

            builder.Property(p => p.Allergies)
                .HasMaxLength(1000);

            builder.Property(p => p.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Email value object configuration
            builder.OwnsOne(p => p.Email, emailBuilder =>
            {
                emailBuilder.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(250);
            });

            // Phone value object configuration
            builder.OwnsOne(p => p.Phone, phoneBuilder =>
            {
                phoneBuilder.Property(p => p.CountryCode)
                    .HasColumnName("PhoneCountryCode")
                    .IsRequired()
                    .HasMaxLength(10);

                phoneBuilder.Property(p => p.Number)
                    .HasColumnName("PhoneNumber")
                    .IsRequired()
                    .HasMaxLength(20);
            });

            // Address value object configuration
            builder.OwnsOne(p => p.Address, addressBuilder =>
            {
                addressBuilder.Property(a => a.Street)
                    .HasColumnName("AddressStreet")
                    .IsRequired()
                    .HasMaxLength(200);

                addressBuilder.Property(a => a.Number)
                    .HasColumnName("AddressNumber")
                    .IsRequired()
                    .HasMaxLength(20);

                addressBuilder.Property(a => a.Complement)
                    .HasColumnName("AddressComplement")
                    .HasMaxLength(100);

                addressBuilder.Property(a => a.Neighborhood)
                    .HasColumnName("AddressNeighborhood")
                    .IsRequired()
                    .HasMaxLength(100);

                addressBuilder.Property(a => a.City)
                    .HasColumnName("AddressCity")
                    .IsRequired()
                    .HasMaxLength(100);

                addressBuilder.Property(a => a.State)
                    .HasColumnName("AddressState")
                    .IsRequired()
                    .HasMaxLength(50);

                addressBuilder.Property(a => a.ZipCode)
                    .HasColumnName("AddressZipCode")
                    .IsRequired()
                    .HasMaxLength(20);

                addressBuilder.Property(a => a.Country)
                    .HasColumnName("AddressCountry")
                    .IsRequired()
                    .HasMaxLength(50);
            });

            // Indexes
            builder.HasIndex(p => new { p.TenantId, p.Document })
                .IsUnique()
                .HasDatabaseName("IX_Patients_TenantId_Document");

            builder.HasIndex(p => p.TenantId)
                .HasDatabaseName("IX_Patients_TenantId");

            builder.HasIndex(p => p.Name)
                .HasDatabaseName("IX_Patients_Name");

            // Relationship with HealthInsurancePlans (0..N)
            builder.HasMany(p => p.HealthInsurancePlans)
                .WithOne(h => h.Patient)
                .HasForeignKey(h => h.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with PatientClinicLinks (N:N through link entity)
            builder.HasMany(p => p.ClinicLinks)
                .WithOne(l => l.Patient)
                .HasForeignKey(l => l.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}