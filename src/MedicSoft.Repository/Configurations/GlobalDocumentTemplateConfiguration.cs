using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class GlobalDocumentTemplateConfiguration : IEntityTypeConfiguration<GlobalDocumentTemplate>
    {
        public void Configure(EntityTypeBuilder<GlobalDocumentTemplate> builder)
        {
            builder.ToTable("GlobalDocumentTemplates");
            
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
                .HasDatabaseName("IX_GlobalDocumentTemplates_TenantId");
                
            builder.HasIndex(x => x.Type)
                .HasDatabaseName("IX_GlobalDocumentTemplates_Type");
                
            builder.HasIndex(x => x.Specialty)
                .HasDatabaseName("IX_GlobalDocumentTemplates_Specialty");
                
            builder.HasIndex(x => x.IsActive)
                .HasDatabaseName("IX_GlobalDocumentTemplates_IsActive");
                
            builder.HasIndex(x => new { x.Name, x.Type, x.TenantId })
                .HasDatabaseName("IX_GlobalDocumentTemplates_Name_Type_TenantId");
        }
    }
}
