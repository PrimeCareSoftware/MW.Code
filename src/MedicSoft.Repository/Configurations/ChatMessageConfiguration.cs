using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.ToTable("ChatMessages");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedNever();

            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(m => m.Type)
                .IsRequired();

            builder.Property(m => m.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationship with Conversation
            builder.HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Sender (User)
            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Self-referencing relationship for ReplyToMessage
            builder.HasOne(m => m.ReplyToMessage)
                .WithMany()
                .HasForeignKey(m => m.ReplyToMessageId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relationship with ReadReceipts
            builder.HasMany(m => m.ReadReceipts)
                .WithOne(r => r.Message)
                .HasForeignKey(r => r.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(m => m.TenantId)
                .HasDatabaseName("IX_ChatMessages_TenantId");

            builder.HasIndex(m => m.ConversationId)
                .HasDatabaseName("IX_ChatMessages_ConversationId");

            builder.HasIndex(m => m.SenderId)
                .HasDatabaseName("IX_ChatMessages_SenderId");

            builder.HasIndex(m => m.SentAt)
                .HasDatabaseName("IX_ChatMessages_SentAt");

            builder.HasIndex(m => new { m.ConversationId, m.SentAt })
                .HasDatabaseName("IX_ChatMessages_ConversationId_SentAt");
        }
    }
}
