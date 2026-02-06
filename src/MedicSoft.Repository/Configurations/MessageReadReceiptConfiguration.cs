using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class MessageReadReceiptConfiguration : IEntityTypeConfiguration<MessageReadReceipt>
    {
        public void Configure(EntityTypeBuilder<MessageReadReceipt> builder)
        {
            builder.ToTable("MessageReadReceipts");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedNever();

            builder.Property(r => r.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationship with Message
            builder.HasOne(r => r.Message)
                .WithMany(m => m.ReadReceipts)
                .HasForeignKey(r => r.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with User
            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(r => r.TenantId)
                .HasDatabaseName("IX_MessageReadReceipts_TenantId");

            builder.HasIndex(r => r.MessageId)
                .HasDatabaseName("IX_MessageReadReceipts_MessageId");

            builder.HasIndex(r => r.UserId)
                .HasDatabaseName("IX_MessageReadReceipts_UserId");

            builder.HasIndex(r => new { r.MessageId, r.UserId })
                .IsUnique()
                .HasDatabaseName("IX_MessageReadReceipts_MessageId_UserId");
        }
    }
}
