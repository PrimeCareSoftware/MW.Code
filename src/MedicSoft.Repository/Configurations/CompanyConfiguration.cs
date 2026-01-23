using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

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

            builder.Property(c => c.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Subdomain)
                .HasMaxLength(63); // DNS subdomain max length

            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Indexes
            builder.HasIndex(c => c.Document)
                .IsUnique()
                .HasDatabaseName("IX_Companies_Document");

            builder.HasIndex(c => c.Subdomain)
                .IsUnique()
                .HasDatabaseName("IX_Companies_Subdomain")
                .HasFilter("[Subdomain] IS NOT NULL"); // Only unique if not null

            builder.HasIndex(c => c.TenantId)
                .HasDatabaseName("IX_Companies_TenantId");

            builder.HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_Companies_IsActive");
        }
    }
}
