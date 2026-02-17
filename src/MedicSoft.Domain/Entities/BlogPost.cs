using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class BlogPost : BaseEntity
    {
        public string Title { get; private set; }
        public string Slug { get; private set; }
        public string Content { get; private set; }
        public string Excerpt { get; private set; }
        public string Category { get; private set; }
        public string? FeaturedImage { get; private set; }
        public int ReadTimeMinutes { get; private set; }
        public bool IsPublished { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public string AuthorName { get; private set; }
        public Guid AuthorId { get; private set; }

        protected BlogPost() { }

        public BlogPost(
            string title,
            string content,
            string excerpt,
            string category,
            int readTimeMinutes,
            string authorName,
            Guid authorId,
            string? featuredImage = null)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Slug = GenerateSlug(title);
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Excerpt = excerpt ?? throw new ArgumentNullException(nameof(excerpt));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            ReadTimeMinutes = readTimeMinutes > 0 ? readTimeMinutes : throw new ArgumentException("Read time must be positive", nameof(readTimeMinutes));
            AuthorName = authorName ?? throw new ArgumentNullException(nameof(authorName));
            AuthorId = authorId;
            FeaturedImage = featuredImage;
            IsPublished = false;
            TenantId = "system"; // Blog posts are system-level, not tenant-specific
        }

        public void Update(
            string title,
            string content,
            string excerpt,
            string category,
            int readTimeMinutes,
            string? featuredImage = null)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Slug = GenerateSlug(title);
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Excerpt = excerpt ?? throw new ArgumentNullException(nameof(excerpt));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            ReadTimeMinutes = readTimeMinutes > 0 ? readTimeMinutes : throw new ArgumentException("Read time must be positive", nameof(readTimeMinutes));
            FeaturedImage = featuredImage;
            UpdateTimestamp();
        }

        public void Publish()
        {
            if (!IsPublished)
            {
                IsPublished = true;
                PublishedAt = DateTime.UtcNow;
                UpdateTimestamp();
            }
        }

        public void Unpublish()
        {
            if (IsPublished)
            {
                IsPublished = false;
                UpdateTimestamp();
            }
        }

        private static string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            // Basic slug generation - convert to lowercase, replace spaces with hyphens
            var slug = title.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("á", "a").Replace("à", "a").Replace("â", "a").Replace("ã", "a")
                .Replace("é", "e").Replace("ê", "e")
                .Replace("í", "i")
                .Replace("ó", "o").Replace("ô", "o").Replace("õ", "o")
                .Replace("ú", "u").Replace("ü", "u")
                .Replace("ç", "c");

            // Remove special characters except hyphens
            return System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");
        }
    }
}
