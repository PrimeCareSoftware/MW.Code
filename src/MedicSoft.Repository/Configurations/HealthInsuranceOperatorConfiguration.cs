using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class HealthInsuranceOperatorConfiguration : IEntityTypeConfiguration<HealthInsuranceOperator>
    {
        public void Configure(EntityTypeBuilder<HealthInsuranceOperator> builder)
        {
            builder.ToTable("HealthInsuranceOperators");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .ValueGeneratedNever();

            builder.Property(o => o.TradeName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.CompanyName)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(o => o.RegisterNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.Document)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.Phone)
                .HasMaxLength(20);

            builder.Property(o => o.Email)
                .HasMaxLength(250);

            builder.Property(o => o.ContactPerson)
                .HasMaxLength(200);

            builder.Property(o => o.WebsiteUrl)
                .HasMaxLength(500);

            builder.Property(o => o.ApiEndpoint)
                .HasMaxLength(500);

            builder.Property(o => o.ApiKey)
                .HasMaxLength(500);

            builder.Property(o => o.TissVersion)
                .HasMaxLength(20);

            builder.Property(o => o.BatchSubmissionEmail)
                .HasMaxLength(250);

            builder.Property(o => o.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(o => new { o.TenantId, o.RegisterNumber })
                .IsUnique()
                .HasDatabaseName("IX_HealthInsuranceOperators_TenantId_RegisterNumber");

            builder.HasIndex(o => o.TenantId)
                .HasDatabaseName("IX_HealthInsuranceOperators_TenantId");

            builder.HasIndex(o => o.TradeName)
                .HasDatabaseName("IX_HealthInsuranceOperators_TradeName");
        }
    }
}
