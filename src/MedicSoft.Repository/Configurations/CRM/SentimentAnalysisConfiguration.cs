using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class SentimentAnalysisConfiguration : IEntityTypeConfiguration<SentimentAnalysis>
    {
        public void Configure(EntityTypeBuilder<SentimentAnalysis> builder)
        {
            builder.ToTable("SentimentAnalyses", schema: "crm");

            builder.HasKey(sa => sa.Id);

            builder.Property(sa => sa.Id)
                .ValueGeneratedNever();

            builder.Property(sa => sa.SourceType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sa => sa.SourceId);

            builder.Property(sa => sa.SourceText)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(sa => sa.Sentiment)
                .IsRequired();

            builder.Property(sa => sa.PositiveScore)
                .IsRequired();

            builder.Property(sa => sa.NegativeScore)
                .IsRequired();

            builder.Property(sa => sa.NeutralScore)
                .IsRequired();

            builder.Property(sa => sa.ConfidenceScore)
                .IsRequired();

            // Store Topics as JSON
            builder.Property<List<string>>("_topics")
                .HasColumnName("Topics")
                .HasColumnType("jsonb");

            builder.Property(sa => sa.AnalyzedAt)
                .IsRequired();

            builder.Property(sa => sa.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sa => sa.CreatedAt)
                .IsRequired();

            builder.Property(sa => sa.UpdatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(sa => sa.SourceType);
            builder.HasIndex(sa => sa.SourceId);
            builder.HasIndex(sa => sa.Sentiment);
            builder.HasIndex(sa => sa.TenantId);
        }
    }
}
