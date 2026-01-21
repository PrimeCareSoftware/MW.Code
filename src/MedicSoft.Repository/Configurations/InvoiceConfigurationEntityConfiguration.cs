using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvoiceConfigEntity = MedicSoft.Domain.Entities.InvoiceConfiguration;

namespace MedicSoft.Repository.Configurations
{
    public class InvoiceConfigurationEntityConfiguration : IEntityTypeConfiguration<InvoiceConfigEntity>
    {
        public void Configure(EntityTypeBuilder<InvoiceConfigEntity> builder)
        {
            builder.ToTable("InvoiceConfigurations");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedNever();

            // Company Info
            builder.Property(c => c.Cnpj)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.MunicipalRegistration)
                .HasMaxLength(50);

            builder.Property(c => c.StateRegistration)
                .HasMaxLength(50);

            builder.Property(c => c.TradeName)
                .HasMaxLength(200);

            // Address
            builder.Property(c => c.Address)
                .HasMaxLength(500);

            builder.Property(c => c.AddressNumber)
                .HasMaxLength(20);

            builder.Property(c => c.AddressComplement)
                .HasMaxLength(200);

            builder.Property(c => c.Neighborhood)
                .HasMaxLength(100);

            builder.Property(c => c.City)
                .HasMaxLength(100);

            builder.Property(c => c.State)
                .HasMaxLength(2);

            builder.Property(c => c.ZipCode)
                .HasMaxLength(10);

            builder.Property(c => c.CityCode)
                .HasMaxLength(10);

            // Contact
            builder.Property(c => c.Phone)
                .HasMaxLength(20);

            builder.Property(c => c.Email)
                .HasMaxLength(200);

            // Tax
            builder.Property(c => c.ServiceCode)
                .HasMaxLength(20);

            builder.Property(c => c.DefaultIssRate)
                .HasColumnType("decimal(5,2)");

            builder.Property(c => c.IssRetainedByDefault)
                .IsRequired();

            builder.Property(c => c.IsSimplifiedTaxRegime)
                .IsRequired();

            builder.Property(c => c.SimplifiedTaxRegimeCode)
                .HasMaxLength(10);

            // Digital Certificate
            builder.Property(c => c.DigitalCertificate)
                .HasColumnType("bytea");

            builder.Property(c => c.CertificatePassword)
                .HasMaxLength(200);

            builder.Property(c => c.CertificateExpirationDate);

            builder.Property(c => c.CertificateThumbprint)
                .HasMaxLength(100);

            // Numbering
            builder.Property(c => c.CurrentInvoiceNumber)
                .IsRequired();

            builder.Property(c => c.DefaultInvoiceSeries)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(c => c.CurrentRpsNumber)
                .IsRequired();

            // Gateway
            builder.Property(c => c.Gateway)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(c => c.GatewayApiKey)
                .HasMaxLength(200);

            builder.Property(c => c.GatewayEnvironment)
                .HasMaxLength(50);

            builder.Property(c => c.IsActive)
                .IsRequired();

            // Automation
            builder.Property(c => c.AutoIssueAfterPayment)
                .IsRequired();

            builder.Property(c => c.SendEmailAfterIssuance)
                .IsRequired();

            builder.Property(c => c.EmailTemplate)
                .HasMaxLength(2000);

            // Base Entity
            builder.Property(c => c.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt);

            // Indexes
            builder.HasIndex(c => c.TenantId)
                .IsUnique()
                .HasDatabaseName("IX_InvoiceConfigurations_TenantId");

            builder.HasIndex(c => c.Cnpj)
                .HasDatabaseName("IX_InvoiceConfigurations_Cnpj");

            builder.HasIndex(c => new { c.TenantId, c.IsActive })
                .HasDatabaseName("IX_InvoiceConfigurations_TenantId_IsActive");
        }
    }
}
