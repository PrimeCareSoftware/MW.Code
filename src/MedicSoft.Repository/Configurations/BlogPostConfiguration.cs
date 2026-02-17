using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
    {
        public void Configure(EntityTypeBuilder<BlogPost> builder)
        {
            builder.ToTable("BlogPosts");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ValueGeneratedNever();

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(b => b.Slug)
                .IsRequired()
                .HasMaxLength(600);

            builder.Property(b => b.Content)
                .IsRequired();

            builder.Property(b => b.Excerpt)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(b => b.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.FeaturedImage)
                .HasMaxLength(500);

            builder.Property(b => b.ReadTimeMinutes)
                .IsRequired();

            builder.Property(b => b.IsPublished)
                .IsRequired();

            builder.Property(b => b.AuthorName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.AuthorId)
                .IsRequired();

            builder.Property(b => b.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(b => b.Slug)
                .IsUnique()
                .HasDatabaseName("IX_BlogPosts_Slug");

            builder.HasIndex(b => b.IsPublished)
                .HasDatabaseName("IX_BlogPosts_IsPublished");

            builder.HasIndex(b => b.Category)
                .HasDatabaseName("IX_BlogPosts_Category");

            builder.HasIndex(b => b.PublishedAt)
                .HasDatabaseName("IX_BlogPosts_PublishedAt");
        }
    }
}
