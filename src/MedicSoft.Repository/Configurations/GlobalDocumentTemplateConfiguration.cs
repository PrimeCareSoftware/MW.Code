using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class GlobalDocumentTemplateConfiguration : IEntityTypeConfiguration<GlobalDocumentTemplate>
    {
        public void Configure(EntityTypeBuilder<GlobalDocumentTemplate> builder)
        {
            // Use lowercase table name for PostgreSQL compatibility
            // PostgreSQL is case-sensitive and treats unquoted identifiers as lowercase
            builder.ToTable("globaldocumenttemplates");
            
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedNever();
                
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(500);
                
            builder.Property(x => x.Type)
                .IsRequired()
                .HasConversion<int>();
                
            builder.Property(x => x.Specialty)
                .IsRequired()
                .HasConversion<int>();
                
            builder.Property(x => x.Content)
                .IsRequired();
                
            builder.Property(x => x.Variables)
                .IsRequired();
                
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
                
            builder.Property(x => x.TenantId)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(x => x.CreatedAt)
                .IsRequired();
                
            builder.Property(x => x.UpdatedAt)
                .IsRequired(false);
                
            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);
            
            // Indexes
            builder.HasIndex(x => x.TenantId)
                .HasDatabaseName("ix_globaldocumenttemplates_tenantid");
                
            builder.HasIndex(x => x.Type)
                .HasDatabaseName("ix_globaldocumenttemplates_type");
                
            builder.HasIndex(x => x.Specialty)
                .HasDatabaseName("ix_globaldocumenttemplates_specialty");
                
            builder.HasIndex(x => x.IsActive)
                .HasDatabaseName("ix_globaldocumenttemplates_isactive");
                
            builder.HasIndex(x => new { x.Name, x.Type, x.TenantId })
                .HasDatabaseName("ix_globaldocumenttemplates_name_type_tenantid");
        }
    }
}
