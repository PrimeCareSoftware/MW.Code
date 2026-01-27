using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
    {
        public void Configure(EntityTypeBuilder<EmailTemplate> builder)
        {
            builder.ToTable("EmailTemplates", schema: "crm");

            builder.HasKey(et => et.Id);

            builder.Property(et => et.Id)
                .ValueGeneratedNever();

            builder.Property(et => et.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(et => et.Subject)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(et => et.HtmlBody)
                .IsRequired()
                .HasMaxLength(8000);

            builder.Property(et => et.PlainTextBody)
                .IsRequired()
                .HasMaxLength(8000);

            builder.Property(et => et.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(et => et.CreatedAt)
                .IsRequired();

            builder.Property(et => et.UpdatedAt)
                .IsRequired();

            // Configure AvailableVariables as JSON
            builder.Property<List<string>>("_availableVariables")
                .HasColumnName("AvailableVariables")
                .HasColumnType("jsonb");

            // Indexes
            builder.HasIndex(et => et.Name);
            builder.HasIndex(et => et.TenantId);
        }
    }
}
