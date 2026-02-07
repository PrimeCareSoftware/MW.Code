using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class DocumentTemplateConfiguration : IEntityTypeConfiguration<DocumentTemplate>
    {
        public void Configure(EntityTypeBuilder<DocumentTemplate> builder)
        {
            builder.ToTable("DocumentTemplates");
            
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
                
            builder.Property(x => x.Specialty)
                .IsRequired()
                .HasConversion<int>();
                
            builder.Property(x => x.Type)
                .IsRequired()
                .HasConversion<int>();
                
            builder.Property(x => x.Content)
                .IsRequired();
                
            builder.Property(x => x.Variables)
                .IsRequired();
                
            builder.Property(x => x.IsActive)
                .IsRequired();
                
            builder.Property(x => x.IsSystem)
                .IsRequired();
                
            builder.Property(x => x.ClinicId)
                .IsRequired(false);
                
            builder.Property(x => x.GlobalTemplateId)
                .IsRequired(false);
                
            builder.Property(x => x.TenantId)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(x => x.CreatedAt)
                .IsRequired();
                
            builder.Property(x => x.UpdatedAt)
                .IsRequired();
            
            // Indexes
            builder.HasIndex(x => x.TenantId)
                .HasDatabaseName("IX_DocumentTemplates_TenantId");
                
            builder.HasIndex(x => x.Specialty)
                .HasDatabaseName("IX_DocumentTemplates_Specialty");
                
            builder.HasIndex(x => x.Type)
                .HasDatabaseName("IX_DocumentTemplates_Type");
                
            builder.HasIndex(x => new { x.TenantId, x.ClinicId })
                .HasDatabaseName("IX_DocumentTemplates_TenantId_ClinicId");
                
            builder.HasIndex(x => x.IsSystem)
                .HasDatabaseName("IX_DocumentTemplates_IsSystem");
            
            // Relationships
            builder.HasOne(x => x.Clinic)
                .WithMany()
                .HasForeignKey(x => x.ClinicId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
                
            builder.HasOne(x => x.GlobalTemplate)
                .WithMany()
                .HasForeignKey(x => x.GlobalTemplateId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
